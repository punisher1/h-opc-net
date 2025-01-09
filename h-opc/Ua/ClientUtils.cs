using Opc.Ua;
using Opc.Ua.Client;

namespace Hylasoft.Opc.Ua
{
    /// <summary>
    /// List of static utility methods
    /// </summary>
    internal static class ClientUtils
    {
        public static ReferenceDescriptionCollection Browse(this Session session, NodeId nodeId)
        {
            var desc = new BrowseDescription
            {
                NodeId = nodeId,
                BrowseDirection = BrowseDirection.Forward,
                IncludeSubtypes = true,
                NodeClassMask = 0U,
                ResultMask = 63U,
            };
            return session.Browse(desc);
        }

        public static ReferenceDescriptionCollection Browse(this Session session, BrowseDescription nodeToBrowse, bool throwOnError = true)
        {
            try
            {
                var descriptionCollection = new ReferenceDescriptionCollection();
                var nodesToBrowse = new BrowseDescriptionCollection { nodeToBrowse };
                session.Browse(null, null, 0U, nodesToBrowse, out BrowseResultCollection results, out DiagnosticInfoCollection diagnosticInfos);
                ClientBase.ValidateResponse(results, nodesToBrowse);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToBrowse);
                while (!StatusCode.IsBad(results[0].StatusCode))
                {
                    for (var index = 0; index < results[0].References.Count; ++index)
                        descriptionCollection.Add(results[0].References[index]);
                    if (results[0].References.Count == 0 || results[0].ContinuationPoint == null)
                        return descriptionCollection;
                    var continuationPoints = new ByteStringCollection
                    {
                        results[0].ContinuationPoint
                    };
                    session.BrowseNext(null, false, continuationPoints, out results, out diagnosticInfos);
                    ClientBase.ValidateResponse(results, continuationPoints);
                    ClientBase.ValidateDiagnosticInfos(diagnosticInfos, continuationPoints);
                }
                throw new ServiceResultException(results[0].StatusCode);
            }
            catch (Exception ex)
            {
                if (throwOnError) throw new ServiceResultException(ex, 2147549184U);
                return null;
            }
        }
    }
}