namespace ExpenseTrackerApi.Helpers
{
    public class JwtSettings
    {
        /// <summary>
        /// Secret key สำหรับสร้างและตรวจสอบ JWT
        /// </summary>
        public string Secret { get; set; } = null!;

        /// <summary>
        /// จำนวนวันก่อนที่ token จะหมดอายุ
        /// </summary>
        public int ExpiresInDay { get; set; }
    }
}
