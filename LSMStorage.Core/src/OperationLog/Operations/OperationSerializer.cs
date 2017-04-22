using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace LSMStorage.Core
{
    public class OperationSerializer : IOperationSerializer
    {
        public byte Marker { get; }
        private static readonly Type IOperationSerializerType = typeof(IOperationSerializer);
        private static readonly IOperationSerializer EmptySerializer = new FakeOperationSerializer();

        private readonly Dictionary<byte, IOperationSerializer> serializersForMarkerMap = new Dictionary<byte, IOperationSerializer>();
        private readonly Dictionary<Type, IOperationSerializer> serializersForTypeMap = new Dictionary<Type, IOperationSerializer>();

        public void AddSerializer<TOperationSerializer>()
            where TOperationSerializer : IOperationSerializer, new()
        {
            var serializer = new TOperationSerializer();
            var baseType = serializer.GetType().BaseType;
            Preconditions.EnsureCondition(baseType != null, "serializerType", $"Should implements 'IOperationSerializer<>'");
            Preconditions.EnsureCondition(baseType != IOperationSerializerType, "serializerType", $"Should implements 'IOperationSerializer<>'");
            Preconditions.EnsureCondition(!serializersForMarkerMap.ContainsKey(serializer.Marker), "serializer", $"Serializer for marker '{serializer.Marker:X}' was added");

            serializersForMarkerMap[serializer.Marker] = serializer;
            serializersForTypeMap[baseType.GenericTypeArguments.First()] = serializer;
        }

        public void AddSerializer<TOperation>(IOperationSerializer<TOperation> serializer)
            where TOperation : IOperation
        {
            Preconditions.EnsureCondition(!serializersForMarkerMap.ContainsKey(serializer.Marker), nameof(serializer), $"Serializer for marker '{serializer.Marker:X}' was added");

            serializersForMarkerMap[serializer.Marker] = serializer;
            serializersForTypeMap[typeof(TOperation)] = serializer;
        }


        public void AddEmptySerializer<TOperation>()
            where TOperation : IOperation
        {
            serializersForTypeMap[typeof(TOperation)] = EmptySerializer;
        }

        public byte[] Serialize(IOperation operation)
        {
            IOperationSerializer serializer;
            if (!serializersForTypeMap.TryGetValue(operation.GetType(), out serializer))
            {
                throw new ApplicationException($"Not found serializer for type '{operation.GetType()}'");
            }
            return serializer.Serialize(operation);
        }

        private IOperationSerializer GetSerializer(Stream stream)
        {
            var result = stream.EnsureReadByte();
            stream.Seek(-1, SeekOrigin.Current);
            IOperationSerializer serializer;
            if (!serializersForMarkerMap.TryGetValue(result, out serializer))
            {
                throw new ApplicationException($"Not found serializer for marker '{result:X}'");
            }
            return serializer;
        }

        public IOperation Deserialize(Stream stream)
        {
            return GetSerializer(stream).Deserialize(stream);
        }

        public Task<IOperation> DeserializeAsync(Stream stream)
        {
            return GetSerializer(stream).DeserializeAsync(stream);
        }
    }
}