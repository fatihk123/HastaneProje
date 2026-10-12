SET XACT_ABORT ON;
IF OBJECT_ID(N'dbo.Tbl_Branslar',N'U') IS NULL
CREATE TABLE dbo.Tbl_Branslar (
 Bransid int IDENTITY(1,1) PRIMARY KEY,
 BransAd nvarchar(100) NOT NULL UNIQUE
);
IF OBJECT_ID(N'dbo.Tbl_Hastalar',N'U') IS NULL
CREATE TABLE dbo.Tbl_Hastalar (
 Hastaid int IDENTITY(1,1) PRIMARY KEY,
 HastaAd nvarchar(100) NOT NULL,
 HastaSoyad nvarchar(100) NOT NULL,
 HastaTc char(11) NOT NULL UNIQUE,
 HastaTelefon varchar(20) NOT NULL,
 HastaSifre nvarchar(256) NOT NULL,
 HastaCinsiyet nvarchar(20) NOT NULL
);
IF OBJECT_ID(N'dbo.Tbl_Doktorlar',N'U') IS NULL
CREATE TABLE dbo.Tbl_Doktorlar (
 Doktorid int IDENTITY(1,1) PRIMARY KEY,
 DoktorAd nvarchar(100) NOT NULL,
 DoktorSoyad nvarchar(100) NOT NULL,
 DoktorBrans nvarchar(100) NOT NULL REFERENCES dbo.Tbl_Branslar(BransAd) ON UPDATE CASCADE,
 DoktorTc char(11) NOT NULL UNIQUE,
 DoktorSifre nvarchar(256) NOT NULL
);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name=N'UX_DoktorTamAd' AND object_id=OBJECT_ID(N'dbo.Tbl_Doktorlar'))
CREATE UNIQUE INDEX UX_DoktorTamAd ON dbo.Tbl_Doktorlar(DoktorAd,DoktorSoyad);
IF OBJECT_ID(N'dbo.Tbl_Sekreter',N'U') IS NULL
CREATE TABLE dbo.Tbl_Sekreter (
 Sekreterid int IDENTITY(1,1) PRIMARY KEY,
 SekreterAdSoyad nvarchar(201) NOT NULL,
 SekreterTc char(11) NOT NULL UNIQUE,
 SekreterSifre nvarchar(256) NOT NULL
);
IF OBJECT_ID(N'dbo.Tbl_Randevular',N'U') IS NULL
CREATE TABLE dbo.Tbl_Randevular (
 Randevuid int IDENTITY(1,1) PRIMARY KEY,
 RandevuTarih date NOT NULL,
 RandevuSaat time(0) NOT NULL,
 RandevuBrans nvarchar(100) NOT NULL REFERENCES dbo.Tbl_Branslar(BransAd) ON UPDATE CASCADE,
 RandevuDoktor nvarchar(201) NOT NULL,
 RandevuDurum bit NOT NULL DEFAULT 0,
 HastaTc char(11) NULL REFERENCES dbo.Tbl_Hastalar(HastaTc),
 HastaSikayet nvarchar(2000) NULL,
 CONSTRAINT CK_RandevuRezervasyon CHECK ((RandevuDurum=0 AND HastaTc IS NULL) OR (RandevuDurum=1 AND HastaTc IS NOT NULL)),
 CONSTRAINT UX_RandevuSaat UNIQUE(RandevuDoktor,RandevuTarih,RandevuSaat)
);
IF OBJECT_ID(N'dbo.Tbl_Duyurular',N'U') IS NULL
CREATE TABLE dbo.Tbl_Duyurular (
 Duyuruid int IDENTITY(1,1) PRIMARY KEY,
 Duyuru nvarchar(2000) NOT NULL
);
