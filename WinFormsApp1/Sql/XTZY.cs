namespace WinFormsApp1.Sql
{
    /// <summary>
    /// 形态转换单（XTZY / AssemVouch）
    /// </summary>
    public static class XTZY
    {
        /// <summary>
        /// 主表 SQL
        /// </summary>
        public static readonly string MainSql = @"
SELECT
    a.ID,
    a.cAVCode AS 单据号,
    a.dAVDate AS 单据日期
FROM AssemVouch a
WHERE
    (@Code IS NULL OR a.cAVCode LIKE @Code)
    AND a.dAVDate >= @StartDate
    AND a.dAVDate <= @EndDate
ORDER BY
    a.dAVDate DESC,
    a.cAVCode DESC;
";

        /// <summary>
        /// 子表 SQL（仅取“转换后”）
        /// </summary>
        public static readonly string DetailSql = @"
SELECT
    CAST(ISNULL(b.cWhCode, '') AS NVARCHAR(50)) AS 仓库编码,
    CAST(ISNULL(wh.cWhName, '') AS NVARCHAR(50)) AS 仓库名称,

    CAST(ISNULL(b.cPosition, '') AS NVARCHAR(50)) AS 货位编码,
    CAST(ISNULL(ps.cPosName, '') AS NVARCHAR(50)) AS 货位名称,

    b.cInvCode        AS 存货编码,
    inv.cInvName        AS 存货名称,

    b.iAVQuantity     AS 数量,
    b.cAVBatch        AS 批号,
    inv.cPackingType  AS 包装方式
FROM AssemVouch a
LEFT JOIN AssemVouchs b
    ON a.ID = b.ID
LEFT JOIN Inventory inv
    ON b.cInvCode = inv.cInvCode
LEFT JOIN Warehouse wh
    ON b.cWhCode = wh.cWhCode
LEFT JOIN Position ps
    ON b.cPosition = ps.cPosCode
WHERE
    a.ID = @MainID
    AND b.bAVType = N'转换后'
ORDER BY
    b.AutoID;
";
    }
}
