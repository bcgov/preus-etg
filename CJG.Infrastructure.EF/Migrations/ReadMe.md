enable-migrations
update-database
update-database -Script -SourceMigration: $InitialDatabase
update-database -Script -SourceMigration: $InitialDatabase -TargetMigration:[name]
update-database -TargetMigration [name]
update-database -TargetMigration:0
add-migration [name]
add-migration [name] -Force 