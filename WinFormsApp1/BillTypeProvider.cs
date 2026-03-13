using System;
using System.Collections.Generic;

namespace WinFormsApp1
{
    public static class BillTypeProvider
    {
        public static List<BillType> GetAll()
        {
            return new List<BillType>
            {
                new BillType { FieldCode = "CGDH",    FieldName = "采购到货单" },
                new BillType { FieldCode = "CGRK",    FieldName = "采购入库单" },
                new BillType { FieldCode = "XSCK",    FieldName = "销售出库单" },
                new BillType { FieldCode = "CCPRK",   FieldName = "产成品入库单" },
                new BillType { FieldCode = "OtherIN", FieldName = "其他入库单" },
                new BillType { FieldCode = "OtherOUT",FieldName = "其他出库单" },
                new BillType { FieldCode = "CLCK",    FieldName = "材料出库单" },
                new BillType { FieldCode = "DBD",     FieldName = "调拨单" },
                new BillType { FieldCode = "PDD",     FieldName = "盘点单" },
                new BillType { FieldCode = "XTZY",    FieldName = "形态转换单" },
                new BillType { FieldCode = "HWTZ",    FieldName = "货位调整单" }
            };
        }
    }
}
