using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Sql
{
    /// <summary>
    /// 调拨单（DBD / TransVouch）
    /// </summary>
    public static class DBD
    {
        /// <summary>
        /// 主表 SQL
        /// </summary>
        public static readonly string MainSql = @"
SELECT
    a.ID,
    a.cTVCode AS 单据号,
    a.dTVDate AS 单据日期
FROM TransVouch a
WHERE
    (@Code IS NULL OR a.cTVCode LIKE @Code)
    AND a.dTVDate >= @StartDate
    AND a.dTVDate <= @EndDate
ORDER BY
    a.dTVDate DESC,
    a.cTVCode DESC;
";

        /// <summary>
        /// 子表 SQL
        /// </summary>
        public static readonly string DetailSql = @"
SELECT
    CAST(ISNULL(a.cIWhCode, '') AS NVARCHAR(50)) AS 仓库编码,
    CAST(ISNULL(wh.cWhName, '') AS NVARCHAR(50)) AS 仓库名称,

    CAST(ISNULL(b.cinposcode, '') AS NVARCHAR(50)) AS 货位编码,
    CAST(ISNULL(ps.cPosName, '') AS NVARCHAR(50)) AS 货位名称,

    b.cInvCode        AS 存货编码,
    inv.cInvName        AS 存货名称,

    b.iTVQuantity     AS 数量,
    b.cTVBatch        AS 批号,
    inv.cPackingType  AS 包装方式
FROM TransVouch a
LEFT JOIN TransVouchs b
    ON a.ID = b.ID
LEFT JOIN Inventory inv
    ON b.cInvCode = inv.cInvCode
LEFT JOIN Warehouse wh
    ON a.cIWhCode = wh.cWhCode
LEFT JOIN Position ps
    ON b.cinposcode = ps.cPosCode
WHERE
    a.ID = @MainID
ORDER BY
    b.AutoID;
";
    }
}

