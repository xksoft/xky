namespace Xky.Core.Model
{
    public class Device
    {
        //加载时序
        public long LoadTick { get; set; }

        /// <summary>
        ///     设备id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     设备序列号
        /// </summary>
        public string Sn { get; set; }

        /// <summary>
        ///     设备名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     坐落的服务器
        /// </summary>
        public string Forward { get; set; }

        /// <summary>
        ///     设备备注
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     节点服务器
        /// </summary>
        public string Node { get; set; }

        /// <summary>
        ///     设备型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        ///     设备厂商
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        ///     连接密钥
        /// </summary>
        public string ConnectionHash { get; set; }

        /// <summary>
        ///     GpsLat
        /// </summary>
        public string GpsLat { get; set; }

        /// <summary>
        ///     GpsLng
        /// </summary>
        public string GpsLng { get; set; }
    }
}