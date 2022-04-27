using System;
using System.Collections.Generic;
using System.Linq;
using MccSoft.DbSyncApp.App.Features.DbScheme.Dto;
using MccSoft.DbSyncApp.App.Features.DbScheme.Enums;
using MccSoft.DbSyncApp.Domain;
using MccSoft.DbSyncApp.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StackExchange.Profiling.Internal;

namespace MccSoft.DbSyncApp.App.Features.DbScheme;

public class DbSchemeService
{
    private readonly DbSyncAppDbContext _dbContext;

    public DbSchemeService(DbSyncAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public DbSchemeDto GetScheme()
    {
        var dbScheme = new DbSchemeDto();

        foreach (IEntityType entityType in _dbContext.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName == null)
            {
                continue;
            }
            var tableIdentifier = StoreObjectIdentifier.Table(tableName, entityType.GetSchema());

            Type entityClassType = entityType.ClrType;
            TableSchemeDto tableScheme =
                new()
                {
                    Name = tableName,
                    EntityFullName = entityClassType.FullName!,
                    AssemblyName = entityClassType.Assembly.FullName!,
                };
            dbScheme.Tables.Add(tableName, tableScheme);

            foreach (IProperty property in entityType.GetProperties())
            {
                var columnName = property.GetColumnName(tableIdentifier);
                if (columnName == null)
                {
                    continue;
                }

                if (property.IsPrimaryKey())
                {
                    tableScheme.PrimaryKeys.Add(columnName);
                }

                var extractedScheme = ExtractAttributeScheme(property);
                if (extractedScheme.Details != null)
                {
                    dbScheme.Schemas.Add(extractedScheme.Details.Name, extractedScheme.Details);
                }

                tableScheme.Attributes.Add(
                    columnName,
                    new AttributeDto
                    {
                        Name = columnName,
                        IsNullable = property.IsColumnNullable(),
                        IsPrimary = property.IsPrimaryKey(),
                        IsUnique = property.IsUniqueIndex(),
                        IsForeignKey = property.IsForeignKey(),
                        Scheme = extractedScheme.Scheme,
                    }
                );
            }

            foreach (IForeignKey ownForeignKey in entityType.GetForeignKeys())
            {
                var connectionInfo = GetEntityConnectionInfoWithOwnForeignKey(
                    ownForeignKey,
                    tableName,
                    entityType
                );
                if (connectionInfo == null)
                {
                    continue;
                }
                tableScheme.Connections.Add(connectionInfo.TableName, connectionInfo);
            }

            foreach (IForeignKey referencedForeignKey in entityType.GetReferencingForeignKeys())
            {
                var connectionInfo = GetEntityConnectionInfoWithReferencedForeignKey(
                    referencedForeignKey,
                    tableName,
                    entityType
                );
                if (connectionInfo == null)
                {
                    continue;
                }
                tableScheme.Connections.Add(connectionInfo.TableName, connectionInfo);
            }
        }

        return dbScheme;
    }

    protected ExtractedTypeDto ExtractAttributeScheme(IProperty property)
    {
        ExtractedTypeDto result = new();

        var attributeScheme = new AttributeSchemeDto();
        result.Scheme = attributeScheme;

        Type type = property.ClrType;
        var typeName = type.Name;

        if (typeName.Contains("Nullable"))
        {
            type = Nullable.GetUnderlyingType(type) ?? property.ClrType;
            typeName = type.Name;
        }

        if (type.Namespace == "System")
        {
            switch (typeName)
            {
                case "String":
                    attributeScheme.Type = RealmDataType.String;
                    break;
                case "Boolean":
                    attributeScheme.Type = RealmDataType.Bool;
                    break;
                case "DateTime":
                    attributeScheme.Type = RealmDataType.Date;
                    attributeScheme.Format = DataFormat.DateTime;
                    break;
                case "DateOnly":
                    attributeScheme.Type = RealmDataType.Date;
                    attributeScheme.Format = DataFormat.Date;
                    break;
                case "TimeOnly":
                    attributeScheme.Type = RealmDataType.Date;
                    attributeScheme.Format = DataFormat.Time;
                    break;
                case "Int32":
                    attributeScheme.Type = RealmDataType.Int;
                    break;
                case "Object":
                    attributeScheme.Type = RealmDataType.Dictionary;
                    attributeScheme.Format = DataFormat.Json;
                    break;
                case "Single":
                case "Double":
                    attributeScheme.Type = RealmDataType.Double;
                    break;
                default:
                    attributeScheme.Type = RealmDataType.String;
                    break;
            }
        }
        else if (type.IsEnum)
        {
            attributeScheme.Ref = $"{nameof(DbSchemeDto.Schemas).ToLower()}/{typeName}";
            result.Details = ExtractEnumFromType(type);
        }

        return result;
    }

    protected SchemeDto ExtractEnumFromType(Type enumType)
    {
        SchemeDto scheme =
            new()
            {
                Name = enumType.Name,
                Enum = new Dictionary<string, int>(),
                AreValuesConfigured = false
            };
        var enumValues = enumType.GetEnumValues();
        for (int i = 0; i < enumValues.Length; i++)
        {
            var name = enumValues.GetValue(i);
            var intValue = Convert.ChangeType(name, Enum.GetUnderlyingType(enumType));
            if (intValue == null || name == null)
            {
                continue;
            }
            if (i != (int)intValue && !scheme.AreValuesConfigured.Value)
            {
                scheme.AreValuesConfigured = true;
            }
            scheme.Enum.Add(name.ToString() ?? string.Empty, (int)intValue);
        }

        return scheme;
    }

    protected ConnectionSchemeDto? GetEntityConnectionInfoWithOwnForeignKey(
        IForeignKey foreignKey,
        string tableName,
        IEntityType entityType
    )
    {
        var connectedEntityType = foreignKey.PrincipalEntityType;
        var connectedTableName = connectedEntityType.GetTableName();
        if (connectedTableName == null)
        {
            return null;
        }

        return new ConnectionSchemeDto()
        {
            TableName = connectedTableName,
            ExternalAttributeNames = GetKeysColumnNames(
                foreignKey.PrincipalKey.Properties,
                connectedTableName,
                connectedEntityType
            ),
            OwnAttributeNames = GetKeysColumnNames(foreignKey.Properties, tableName, entityType),
        };
    }

    protected ConnectionSchemeDto? GetEntityConnectionInfoWithReferencedForeignKey(
        IForeignKey foreignKey,
        string tableName,
        IEntityType entityType
    )
    {
        var connectedEntityType = foreignKey.DeclaringEntityType;
        var connectedTableName = connectedEntityType.GetTableName();
        if (connectedTableName == null)
        {
            return null;
        }

        return new ConnectionSchemeDto()
        {
            TableName = connectedTableName,
            ExternalAttributeNames = GetKeysColumnNames(
                foreignKey.Properties,
                connectedTableName,
                connectedEntityType
            ),
            OwnAttributeNames = GetKeysColumnNames(
                foreignKey.PrincipalKey.Properties,
                tableName,
                entityType
            ),
            isIncomingReference = true,
        };
    }

    private List<string> GetKeysColumnNames(
        IEnumerable<IProperty> properties,
        string tableName,
        IEntityType entityType
    )
    {
        var tableIdentifier = StoreObjectIdentifier.Table(tableName, entityType.GetSchema());
        return properties
            .Select(property => property.GetColumnName(tableIdentifier) ?? property.Name)
            .ToList();
    }
}
