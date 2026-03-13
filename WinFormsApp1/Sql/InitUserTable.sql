-- 创建用户表 LabelUser_yx
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LabelUser_yx]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LabelUser_yx](
        [ID] [int] IDENTITY(1,1) NOT NULL,
        [User] [nvarchar](50) NOT NULL,
        [Password] [nvarchar](50) NOT NULL,
        [Enable] [bit] NOT NULL DEFAULT 1,
        CONSTRAINT [PK_LabelUser_yx] PRIMARY KEY CLUSTERED 
        (
            [ID] ASC
        )
    )
END
GO

-- 插入默认用户 pp (如果不存在)
IF NOT EXISTS (SELECT 1 FROM [dbo].[LabelUser_yx] WHERE [User] = 'pp')
BEGIN
    INSERT INTO [dbo].[LabelUser_yx] ([User], [Password], [Enable])
    VALUES ('pp', '123', 1)
END
GO
