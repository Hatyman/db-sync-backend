rem This is a .bat file to generate code coverage reports.
rem Download coverage.cobertura.xml files from build Artifacts (backend_coverage folder) and put it to `./cobertura` folder.
rem Then run coverage.bat and check `./cobertura/report` folder to see the coverage.
rem You could apply filters to see the coverage on separate files/classes.

dotnet reportgenerator -reports:cobertura/* -targetdir:cobertura/report -sourcedirs:./ -assemblyfilters:+MccSoft.DbSyncApp.App;+MccSoft.DbSyncApp.Common;+MccSoft.DbSyncApp.Domain
open cobertura/report/index.html
