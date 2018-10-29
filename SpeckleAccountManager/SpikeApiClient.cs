//using System.CodeDom.Compiler;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Runtime.CompilerServices;
//using System.Runtime.Serialization;
//using System.Threading;
//using System.Threading.Tasks;

//namespace SpikePopup
//{
//    public class SpikeApiClient : ISerializable
//    {
//        public SpikeApiClient(bool useGzip = true)
//        {

//        }
//        public SpikeApiClient(string baseUrl, bool isPersistent = false)
//        {

//        }
//        protected SpikeApiClient(SerializationInfo info, StreamingContext context)
//        {

//        }
        
//        public string AuthToken { get; set; }
//        public string BaseUrl { get; set; }
//        public bool UseGzip { get; set; }
//        public User User { get; }
//        public string StreamId { get; }
//        public string ClientId { get; }
//        public bool IsAuthorized { get; }
//        public bool IsPersistent { get; }
//        public bool WsConnected { get; }
//        public bool IsDisposed { get; }
//        public bool IsConnected { get; }

//        public void GetObjectData(SerializationInfo info, StreamingContext context)
//        {
//            throw new System.NotImplementedException();
//        }

//        public Task<ResponseUser> UserLoginAsync(User body, CancellationToken cancellationToken)
//        {

//        }
//        public Task<ResponseUser> UserLoginAsync(User body)
//        {

//        }
//        public Task<ResponseUser> UserRegisterAsync(User body, CancellationToken cancellationToken)
//        {

//        }
//        public Task<ResponseUser> UserRegisterAsync(User body)
//        {

//        }
//    }
//}
