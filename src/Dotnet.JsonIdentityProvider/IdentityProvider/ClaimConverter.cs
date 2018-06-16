namespace Dotnet.JsonIdentityProvider.IdentityProvider
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ClaimConverter : JsonConverter
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Claim));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load the JSON for the Result into a JObject
            JObject jo = JObject.Load(reader);

            // Read the properties which will be used as constructor parameters
            var type = (string)jo["Type"];
            var value = (string)jo["Value"];
            var valueType = (string)jo["ValueType"];
            var issuer = (string)jo["Issuer"];
            var originalIssuer = (string)jo["OriginalIssuer"];

            // Construct the Result object using the non-default constructor
            Claim result = new Claim(type, value, valueType, issuer, originalIssuer);

            // Return the result
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}