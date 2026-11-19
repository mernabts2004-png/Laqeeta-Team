# Laqeeta Report Queries

Table names below are the EF Core defaults (Stations, Items, Reports, Claims). Check them in SQL Server if they differ.

## Query 1: Found and returned items per station (implemented)

Used by GET /api/AdminReports/stations.
In plain words: for each station, count the items found there, count how many were returned to their owners, and calculate the return rate.

SELECT
    s.StationId,
    s.Name AS StationName,
    COUNT(i.ItemId) AS FoundItemsCount,
    SUM(CASE WHEN i.Status = N'Returned' THEN 1 ELSE 0 END) AS ReturnedItemsCount,
    CASE WHEN COUNT(i.ItemId) = 0 THEN 0
         ELSE CAST(SUM(CASE WHEN i.Status = N'Returned' THEN 1 ELSE 0 END) AS decimal(10,2)) * 100
              / COUNT(i.ItemId)
    END AS ReturnRate
FROM Stations s
LEFT JOIN Items i ON i.FoundAtStationId = s.StationId
GROUP BY s.StationId, s.Name
ORDER BY s.Name;
## Query 2: Found and returned items per category (documented only)

In plain words: which kinds of items are found most, and which kinds actually return to their owners.

SELECT
    Category,
    COUNT(*) AS FoundItemsCount,
    SUM(CASE WHEN Status = N'Returned' THEN 1 ELSE 0 END) AS ReturnedItemsCount
FROM Items
GROUP BY Category
ORDER BY FoundItemsCount DESC;
## Query 3: Items still waiting after 30 days (documented only)

In plain words: items found more than 30 days ago that are not returned yet, with their current location. Items marked Reserved are waiting for their owner.

SELECT
    i.ItemId,
    i.Category,
    s.Name AS StationName,
    i.StorageLocation,
    i.Status,
    DATEDIFF(DAY, i.FoundAt, SYSUTCDATETIME()) AS DaysSinceFound
FROM Items i
JOIN Stations s ON s.StationId = i.FoundAtStationId
WHERE i.Status <> N'Returned'
  AND i.FoundAt <= DATEADD(DAY, -30, SYSUTCDATETIME())
ORDER BY DaysSinceFound DESC;