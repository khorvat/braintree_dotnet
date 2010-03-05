﻿using System;
using System.Collections.Generic;
using System.Xml;

namespace Braintree
{
    /// <summary>
    /// Provides operations for creating, finding, updating, and deleting customers in the vault
    /// </summary>
    public class CustomerGateway
    {
        public virtual Customer Find(String Id)
        {
            XmlNode customerXML = WebServiceGateway.Get("/customers/" + Id);

            return new Customer(new NodeWrapper(customerXML));
        }

        public virtual Result<Customer> Create(CustomerRequest request)
        {
            XmlNode customerXML = WebServiceGateway.Post("/customers", request);

            return new Result<Customer>(new NodeWrapper(customerXML));
        }

        public virtual void Delete(String Id)
        {
            WebServiceGateway.Delete("/customers/" + Id);
        }

        public virtual Result<Customer> Update(String Id, CustomerRequest request)
        {
            XmlNode customerXML = WebServiceGateway.Put("/customers/" + Id, request);

            return new Result<Customer>(new NodeWrapper(customerXML));
        }

        public virtual Result<Customer> ConfirmTransparentRedirect(String queryString)
        {
            TransparentRedirectRequest trRequest = new TransparentRedirectRequest(queryString);
            XmlNode node = WebServiceGateway.Post("/customers/all/confirm_transparent_redirect_request", trRequest);

            return new Result<Customer>(new NodeWrapper(node));
        }

        public virtual String TransparentRedirectURLForCreate()
        {
            return Configuration.BaseMerchantURL() + "/customers/all/create_via_transparent_redirect_request";
        }

        public virtual String TransparentRedirectURLForUpdate()
        {
            return Configuration.BaseMerchantURL() + "/customers/all/update_via_transparent_redirect_request";
        }

        public PagedCollection<Customer> All()
        {
            return All(1);
        }

        public PagedCollection<Customer> All(Int32 pageNumber)
        {
            String queryString = new QueryString().Append("page", pageNumber).ToString();
            NodeWrapper response = new NodeWrapper(WebServiceGateway.Get("/customers?" + queryString));
            
            int currentPageNumber = response.GetInteger("current-page-number").Value;
            int pageSize = response.GetInteger("page-size").Value;
            int totalItems = response.GetInteger("total-items").Value;

            List<Customer> customers = new List<Customer>();
            foreach (NodeWrapper node in response.GetList("customer"))
            {
                customers.Add(new Customer(node));
            }

            return new PagedCollection<Customer>(customers, currentPageNumber, totalItems, pageSize, delegate() { return All(pageNumber + 1); });
        }
    }
}