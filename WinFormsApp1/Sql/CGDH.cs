using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Sql
{
    /// <summary>
    /// 采购到货单（CGDH）
    /// </summary>
    public static class CGDH
    {
        /// <summary>
        /// 主表 SQL
        /// 表：PU_ArrivalVouch
        /// </summary>
        public static readonly string MainSql = @"
SELECT
    v.ID,
    v.cCode  AS 单据号,
    v.dDate  AS 单据日期
FROM PU_ArrivalVouch v
WHERE
    (@Code IS NULL OR v.cCode LIKE @Code)
    AND v.dDate >= @StartDate
    AND v.dDate <= @EndDate
ORDER BY
    v.dDate DESC, v.cCode DESC
";

        /// <summary>
        /// 子表 SQL
        /// 表：PU_ArrivalVouchs
        /// </summary>
        public static readonly string DetailSql = @"
SELECT
CAST(a.cWhCode AS NVARCHAR(50))            AS 仓库编码,
CAST(wh.cWhName AS NVARCHAR(50))           AS 仓库名称,

CAST('' AS NVARCHAR(50))           AS 货位编码,
CAST('' AS NVARCHAR(50))          AS 货位名称,

    a.cInvCode             AS 存货编码,
    inv.cInvName           AS 存货名称,

    a.iQuantity            AS 数量,
    a.cBatch               AS 批号,
    inv.cPackingType       AS 包装方式
FROM PU_ArrivalVouchs a
LEFT JOIN Inventory inv
    ON a.cInvCode = inv.cInvCode
LEFT JOIN Warehouse wh
    ON a.cWhCode = wh.cWhCode
WHERE
    a.ID = @MainID
ORDER BY
    a.AutoID;

";
    }
}

