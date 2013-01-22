#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Xml;
using Braintree.Exceptions;

namespace Braintree
{
    /// <summary>
    /// Provides operations for finding verifications
    /// </summary>
    public class CreditCardVerificationGateway
    {
        private BraintreeService Service;

        protected internal CreditCardVerificationGateway(BraintreeService service)
        {
            Service = service;
        }

        public virtual CreditCardVerification Find(String Id)
        {
            if(Id == null || Id.Trim().Equals(""))
                throw new NotFoundException();

            XmlNode creditCardVerificationXML = Service.Get("/verifications/" + Id);

            return new CreditCardVerification(new NodeWrapper(creditCardVerificationXML), Service);
        }

        public virtual ResourceCollection<CreditCardVerification> Search(CreditCardVerificationSearchRequest query)
        {
            NodeWrapper response = new NodeWrapper(Service.Post("/verifications/advanced_search_ids", query));

            return new ResourceCollection<CreditCardVerification>(response, delegate(String[] ids) {
                return FetchCreditCardVerifications(query, ids);
            });
        }

        private List<CreditCardVerification> FetchCreditCardVerifications(CreditCardVerificationSearchRequest query, String[] ids)
        {
            query.Ids.IncludedIn(ids);

            NodeWrapper response = new NodeWrapper(Service.Post("/verifications/advanced_search", query));

            List<CreditCardVerification> verifications = new List<CreditCardVerification>();
            foreach (NodeWrapper node in response.GetList("verification"))
            {
                verifications.Add(new CreditCardVerification(node, Service));
            }
            return verifications;
        }
    }
}
