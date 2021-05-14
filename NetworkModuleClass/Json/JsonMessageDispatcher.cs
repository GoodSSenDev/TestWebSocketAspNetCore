#nullable enable

using System;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NetworkModule.Json
{
    /// <summary>
    /// Represent Json version of messageDispatcher
    /// </summary>
    public class JsonMessageDispatcher : MessageDispatcher<JObject>
    {

        protected override TMessage Deserialize<TMessage>( JObject message )
             => JsonSerialization.Deserialize<TMessage>( message );

        protected override object Deserialize( Type paramType, JObject message )
            => JsonSerialization.ToObject( paramType, message );

        protected override RouteAttribute? GetRouteAttribute( MethodInfo mi )
            => mi.GetCustomAttribute<JsonRouteAttribute>( );

        //protected override async Task<JObject?> DispatchAsync(JObject message)
        //{ 

        //    if (message.GetType().GetProperty("DispatchIndex").GetValue(null) is int dispatchIndex)
        //    {
        //        return await _handlers[dispatchIndex].targetMethod(message);
        //    }

        //    //foreach (var (route, target) in _handlers)
        //    //{
        //    //    if (IsMatch(route, message))
        //    //    {
        //    //        return await target(message);
        //    //    }
        //    //}

        //    //No handler?? what to do??
        //    return null;
        //}

        protected override bool IsMatch( RouteAttribute route, JObject message )
            => message.SelectToken( route.Path )?.ToString( ) == ( route as JsonRouteAttribute )?.Value;

        protected override JObject? Serialize<TMessage>( TMessage instance )
            => JsonSerialization.Serialize( instance );
    }
}
