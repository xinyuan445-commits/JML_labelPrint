using System;

namespace WinFormsApp1.Sql
{
    /// <summary>
    /// 单据 SQL 工厂（简单版）
    /// </summary>
    public static class BillSqlFactory
    {
        public static string GetMainSql(string fieldCode)
        {
            return fieldCode switch
            {
                "CGDH" => CGDH.MainSql,
                "CGRK" => CGRK.MainSql,
                "OtherIN" => OtherIN.MainSql,
                "OtherOUT" => OtherOUT.MainSql,
                "CCPRK" => CCPRK.MainSql,
                "CLCK" => CLCK.MainSql,
                "XSCK" => XSCK.MainSql,
                "DBD" => DBD.MainSql,
                "XTZY" => XTZY.MainSql,
                "HWTZ" => HWTZ.MainSql,

                _ => throw new NotSupportedException($"未支持的单据主表：{fieldCode}")
            };
        }

        public static string GetDetailSql(string fieldCode)
        {
            return fieldCode switch
            {
                "CGDH" => CGDH.DetailSql,
                "CGRK" => CGRK.DetailSql,
                "OtherIN" => OtherIN.DetailSql,
                "OtherOUT" => OtherOUT.DetailSql,
                "CCPRK" => CCPRK.DetailSql,
                "CLCK" => CLCK.DetailSql,
                "XSCK" => XSCK.DetailSql,
                "DBD" => DBD.DetailSql,
                "XTZY" => XTZY.DetailSql,
                "HWTZ" => HWTZ.DetailSql,

                _ => throw new NotSupportedException($"未支持的单据子表：{fieldCode}")
            };
        }

    }
}
