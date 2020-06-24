CREATE PROCEDURE AddSetting @SettingId VARCHAR(255), @SettingValue VARCHAR(255), @Help VARCHAR(1000), @IsAdmin BIT
AS
BEGIN
   IF NOT EXISTS (SELECT SettingId FROM dbo.Settings WHERE SettingId = @SettingId)
   BEGIN
       INSERT INTO dbo.Settings (SettingId, SettingValue, Help, IsAdmin)
       VALUES (@SettingId, @SettingValue, @Help, @IsAdmin);
   END
END