# 标签打印系统 (Label Print System)

## 项目简介

本项目是一个基于 **.NET 8.0 (WinForms)** 开发的标签打印管理系统，旨在解决生产/仓储环节的条码标签打印需求。系统能够直接对接 **U8 ERP** 数据库，检索采购入库、销售出库等业务单据，并驱动 **CodeSoft** 软件批量生成和打印带有追溯信息的条码标签。

## 核心功能

*   **单据对接**: 支持多种 U8 业务单据查询（如采购到货、采购入库、销售出库、材料出库等）。
*   **智能打印**: 
    *   自动计算标签数量（根据包装方式自动拆箱）。
    *   自动生成唯一追溯流水号。
    *   调用 CodeSoft 模板进行动态数据填充与打印。
*   **规则配置**: 
    *   **条码规则**: 支持自定义条码解析与生成规则 (Form2)。
    *   **单据筛选**: 可配置前台显示的单据类型 (Form3)。
*   **数据追溯**: 打印记录自动回写至数据库 (`TraceBackOrder`, `TraceBackGoods`, `TraceBackOnlyCode`)，实现一物一码追溯。

## 技术栈

*   **开发框架**: .NET 8.0 (Windows Desktop)
*   **UI 框架**: Windows Forms
*   **数据库**: SQL Server (Microsoft.Data.SqlClient)
*   **打印引擎**: CodeSoft (LabelManager2 COM Interop)
*   **配置存储**: JSON

## 环境要求

1.  **操作系统**: Windows 10/11 (需支持 .NET 8)。
2.  **运行时**: [.NET Desktop Runtime 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)。
3.  **打印软件**: 必须安装 **CodeSoft (LabelManager2)** 并确保 COM 组件已注册。
4.  **数据库**: 能够访问 U8 ERP 的 SQL Server 数据库。

## 项目结构

```
WinFormsApp1/
├── Form1.cs                  # 主界面：单据查询与打印控制
├── Form2.cs                  # 规则配置界面
├── Form3.cs                  # 单据类型选择配置
├── Services/
│   └── CodeSoftLabelPrintService.cs # 封装 CodeSoft COM 调用的打印服务
├── Sql/                      # SQL 工厂模式实现
│   ├── BillSqlFactory.cs     # SQL 工厂类
│   └── [CGDH.cs, XSCK.cs...] # 各类单据的具体 SQL 实现
├── Program.cs                # 程序入口 (含数据库连接配置)
└── 100X180mm/                # 存放 .lab 标签模板文件
```

## 快速开始

1.  **配置数据库连接**:
    *   打开 `Program.cs`。
    *   修改 `SqlServerDb.ConnectionString` 中的数据库连接字符串（Server, Database, User Id, Password）。
    
    > **注意**: 建议在正式部署时将连接字符串移至配置文件。

2.  **配置打印模板**:
    *   确保 `.lab` 模板文件路径正确（默认在 `100X180mm/print.Lab`）。
    *   模板变量需与代码中的变量名对应（如：`物料编码`, `数量`, `条码` 等）。

3.  **运行程序**:
    *   使用 Visual Studio 2022 打开解决方案。
    *   编译并运行。
    *   在主界面选择单据类型，查询并打印。

## 开发注意事项

*   **COM 组件释放**: 
    *   项目已针对 CodeSoft 的 COM 对象实现了显式释放逻辑 (`Marshal.ReleaseComObject`)，防止打印服务进程 (`lppa.exe`) 卡死或内存泄漏。
    *   修改 `CodeSoftLabelPrintService.cs` 时请务必保留 `finally` 块中的清理代码。
*   **SQL 扩展**: 
    *   如需支持新单据，请在 `Sql/` 目录下新建类（继承标准结构），并在 `BillSqlFactory.cs` 中注册。

## 许可证

Private / Proprietary
