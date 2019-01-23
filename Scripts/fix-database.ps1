$database = Get-Item ".\main.db"
$connectionString = "Data Source=$($database.FullName);Version=3;"

$tableData = Invoke-SqliteQuery -Sql "SELECT * FROM SongInfo;" -ConnectionString $connectionString
$groupedData = $tableData | Group-Object SongID | Select-Object @{
    Name = "tmp";
    Expression = {
        [PSCustomObject] @{
            ID = 0
            SongID = $_.Group.SongID | Select-Object -First 1
            LastPlayed = [long]($_.Group | Measure-Object LastPlayed -Maximum).Maximum
            PlayCount = [long]($_.Group | Measure-Object PlayCount -Sum).Sum
        }
    }
} | Select-Object -ExpandProperty "tmp"

$i = 1
foreach($data in $groupedData)
{
    $data.ID = $i
    $i++
}

Invoke-SqliteQuery -Sql "DROP TABLE SongInfo;" -ConnectionString $connectionString
Invoke-SqliteQuery -Sql 'CREATE TABLE "SongInfo" ( "ID" integer primary key autoincrement not null , "LastPlayed" bigint , "PlayCount" integer , "SongID" integer )' -ConnectionString $connectionString

foreach($data in $groupedData)
{
    Invoke-SqliteQuery -Sql "INSERT INTO SongInfo VALUES($($data.ID), $($data.LastPlayed), $($data.PlayCount), $($data.SongID));" -ConnectionString $connectionString -CUD
}
