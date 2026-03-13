using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Sql
{
    /// <summary>
    /// 货位调整单（HWTZ / AdjustPVouch）
    /// </summary>
    public static class HWTZ
    {
        /// <summary>
        /// 主表 SQL
        /// </summary>
        public static readonly string MainSql = @"
SELECT
    a.ID,
    a.cVouchCode AS 单据号,
    a.dDate      AS 单据日期
FROM AdjustPVouch a
WHERE
    (@Code IS NULL OR a.cVouchCode LIKE @Code)
    AND a.dDate >= @StartDate
    AND a.dDate <= @EndDate
ORDER BY
    a.dDate DESC,
    a.cVouchCode DESC;
";

        /// <summary>
        /// 子表 SQL
        /// </summary>
        public static readonly string DetailSql = @"
SELECT
    CAST(ISNULL(a.cWhCode, '') AS NVARCHAR(50)) AS 仓库编码,
    CAST(ISNULL(wh.cWhName, '') AS NVARCHAR(50)) AS 仓库名称,

    CAST(ISNULL(b.cAPosCode, '') AS NVARCHAR(50)) AS 货位编码,
    CAST(ISNULL(ps.cPosName, '') AS NVARCHAR(50)) AS 货位名称,

    b.cInvCode       AS 存货编码,
    inv.cInvName       AS 存货名称,

    b.iQuantity      AS 数量,
    b.cBatch         AS 批号,
    inv.cPackingType AS 包装方式
FROM AdjustPVouch a
LEFT JOIN AdjustPVouchs b
    ON a.ID = b.ID
LEFT JOIN Inventory inv
    ON b.cInvCode = inv.cInvCode
LEFT JOIN Warehouse wh
    ON a.cWhCode = wh.cWhCode
LEFT JOIN Position ps
    ON b.cAPosCode = ps.cPosCode
WHERE
    a.ID = @MainID
ORDER BY
    b.AutoID;
";
    }
}

