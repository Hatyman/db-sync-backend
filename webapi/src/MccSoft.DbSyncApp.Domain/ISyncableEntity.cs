using MccSoft.DomainHelpers;

namespace MccSoft.DbSyncApp.Domain;

public interface ISyncableEntity
{
    public string Id { get; set; }
    public static Specification<T> HasId<T>(string id) where T : ISyncableEntity =>
        new(nameof(HasId), p => p.Id == id, id);
    public static Specification<ISyncableEntity> HasId(string id) =>
        new(nameof(HasId), p => p.Id == id, id);
}
