using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Sql
{
    /// <summary>
    /// 采购入库单（CGRK）SQL
    /// </summary>
    public static class CGRK
    {
        /// <summary>
        /// 主表 SQL
        /// </summary>
        public static readonly string MainSql = @"
SELECT
    r.ID,
    r.cCode AS 单据号,
    r.dDate AS 单据日期
FROM RdRecord01 r
WHERE
    (@Code IS NULL OR r.cCode LIKE @Code)
    AND r.dDate >= @StartDate
    AND r.dDate <= @EndDate
ORDER BY
    r.dDate DESC,
    r.cCode DESC;
";

        /// <summary>
        /// 子表 SQL
        /// </summary>
        public static readonly string DetailSql = @"
SELECT
    CAST(ISNULL(a.cWhCode, '') AS NVARCHAR(50)) AS 仓库编码,
    CAST(ISNULL(a.cWhName, '') AS NVARCHAR(50)) AS 仓库名称,

    CAST(ISNULL(a.cPosition, '') AS NVARCHAR(50)) AS 货位编码,
    CAST(ISNULL(a.cPosName, '') AS NVARCHAR(50)) AS 货位名称,

    a.cInvCode       AS 存货编码,
    a.cInvName       AS 存货名称,

    a.iQuantity      AS 数量,
    a.cBatch         AS 批号,
    inv.cPackingType AS 包装方式
FROM rdall_yx a
LEFT JOIN Inventory inv
    ON a.cInvCode = inv.cInvCode
WHERE
    a.Class = '01'
    AND a.ID = @MainID
ORDER BY
    a.AutoID;
";
    }
}
