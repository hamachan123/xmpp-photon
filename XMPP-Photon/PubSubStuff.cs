﻿using System;
using System.Net;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace com.negociosit.XMPP.XMPPphoton
{
    public class PubSubOperation
    {

        static string QueryNodeXML =
  @"<query xmlns='http://jabber.org/protocol/disco#info' node='#NODE#' />";

        /// <summary>
        /// Checks to see if a given node exists
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="strNode"></param>
        /// <returns></returns>
        public static bool NodeExists(XMPPClient connection, string strNode)
        {
            string strXML = QueryNodeXML.Replace("#NODE#", strNode);
            IQ IQRequest = new IQ();
            IQRequest.From = connection.JID;
            IQRequest.To = string.Format("pubsub.{0}", connection.Domain);
            IQRequest.InnerXML = strXML;
            IQ IQResponse = connection.SendRecieveIQ(IQRequest, 10000);


            if (IQResponse == null)
                return false;
            if (IQResponse.Type == IQType.error.ToString()) // && (IQResponse.Error.Code >= 0))
            {
                return false;
            }

            return true;
        }

        static string FetchChildNodes =  @"<query xmlns='http://jabber.org/protocol/disco#items' node='#NODE#' />";
        public static string[] GetSubNodes(XMPPClient connection, string strNode)
        {
            List<string> SubNodeList = new List<string>();
            string strXML = FetchChildNodes.Replace("#NODE#", strNode);

            ServiceDiscoveryIQ IQRequest = new ServiceDiscoveryIQ();
            IQRequest.ServiceDiscoveryItemQuery = new ServiceDiscoveryItemQuery();
            IQRequest.From = connection.JID;
            IQRequest.To = string.Format("pubsub.{0}", connection.Domain);
            IQRequest.Type = IQType.get.ToString();
            IQRequest.ServiceDiscoveryItemQuery.Node = strNode;

            IQ IQResponse = connection.SendRecieveIQ(IQRequest, 10000, SerializationMethod.XMLSerializeObject);

            if (IQResponse == null)
                return null;

            if (IQResponse.Type == IQType.error.ToString())
            {
                return SubNodeList.ToArray();
            }

            if (IQResponse is ServiceDiscoveryIQ)
            {
                ServiceDiscoveryIQ sdiq = IQResponse as ServiceDiscoveryIQ;
                if ((sdiq.ServiceDiscoveryItemQuery != null) && (sdiq.ServiceDiscoveryItemQuery.Items != null) )
                {
                    foreach (item it in sdiq.ServiceDiscoveryItemQuery.Items)
                    {
                        SubNodeList.Add(it.Node);
                    }
                }
            }

            //var nodes = IQResponse.InitalXMLElement.Descendants("{http://jabber.org/protocol/disco#items}item");
            //foreach (XElement elem in nodes)
            //{
            //    XAttribute attrnode = elem.Attribute("node");
            //    if (attrnode != null)
            //       SubNodeList.Add(attrnode.Value);
            //}
           

            return SubNodeList.ToArray();
        }

        public static item[] GetSubNodeItems(XMPPClient connection, string strNode)
        {
            List<item> SubNodeList = new List<item>();
            ServiceDiscoveryIQ IQRequest = new ServiceDiscoveryIQ();
            IQRequest.ServiceDiscoveryItemQuery = new ServiceDiscoveryItemQuery();
            IQRequest.From = connection.JID;
            IQRequest.To = string.Format("pubsub.{0}", connection.Domain);
            IQRequest.Type = IQType.get.ToString();
            IQRequest.ServiceDiscoveryItemQuery.Node = strNode;

            IQ IQResponse = connection.SendRecieveIQ(IQRequest, 10000, SerializationMethod.XMLSerializeObject);

            if (IQResponse == null)
                return null;

            if (IQResponse.Type == IQType.error.ToString())
            {
                return SubNodeList.ToArray();
            }

            if (IQResponse is ServiceDiscoveryIQ)
            {
                ServiceDiscoveryIQ sdiq = IQResponse as ServiceDiscoveryIQ;
                if ((sdiq.ServiceDiscoveryItemQuery != null) && (sdiq.ServiceDiscoveryItemQuery.Items != null))
                {
                    foreach (item it in sdiq.ServiceDiscoveryItemQuery.Items)
                    {
                        SubNodeList.Add(it);
                    }
                }
            }

            return SubNodeList.ToArray();
        }


        static string FetchChildNodesRoot = @"<query xmlns='http://jabber.org/protocol/disco#items'/>";

        public static string[] GetRootNodes(XMPPClient connection)
        {
            List<string> SubNodeList = new List<string>();
            ServiceDiscoveryIQ IQRequest = new ServiceDiscoveryIQ();
            IQRequest.ServiceDiscoveryItemQuery = new ServiceDiscoveryItemQuery();
            IQRequest.From = connection.JID;
            IQRequest.To = string.Format("pubsub.{0}", connection.Domain);
            IQRequest.Type = IQType.get.ToString();

            IQ IQResponse = connection.SendRecieveIQ(IQRequest, 10000, SerializationMethod.XMLSerializeObject);

            if (IQResponse == null)
                return null;

            if (IQResponse.Type == IQType.error.ToString())
            {
                return SubNodeList.ToArray();
            }

            if (IQResponse is ServiceDiscoveryIQ)
            {
                ServiceDiscoveryIQ sdiq = IQResponse as ServiceDiscoveryIQ;
                if ( (sdiq.ServiceDiscoveryItemQuery != null) && (sdiq.ServiceDiscoveryItemQuery.Items != null))
                {
                    foreach (item it in sdiq.ServiceDiscoveryItemQuery.Items)
                    {
                        SubNodeList.Add(it.Node);
                    }
                }
            }
            //var nodes = IQResponse.InitalXMLElement.Descendants("{http://jabber.org/protocol/disco#items}item");
            //foreach (XElement elem in nodes)
            //{
            //    XAttribute attrnode = elem.Attribute("node");
            //    SubNodeList.Add(attrnode.Value);
            //}
           

            return SubNodeList.ToArray();
        }


        public static PubSubItem[] GetNodeItems(XMPPClient connection, string strNode, out string strNodeJID)
        {
            strNodeJID = "";
            List<PubSubItem> returnnodes = new List<PubSubItem>();
            PubSubIQ IQRequest = new PubSubIQ();
            IQRequest.Type = IQType.get.ToString();
            IQRequest.From = connection.JID;
            IQRequest.To = string.Format("pubsub.{0}", connection.Domain);
            IQRequest.PubSub = new PubSub();
            IQRequest.PubSub.Items = new PubSubItems();
            IQRequest.PubSub.Items.Node = strNode;
            IQ IQResponse = connection.SendRecieveIQ(IQRequest, 30000, SerializationMethod.XMLSerializeObject);

            if (IQResponse.Type == IQType.error.ToString())
            {
                return returnnodes.ToArray();
            }

            if (IQResponse is PubSubIQ)
            {
                PubSubIQ psiq = IQResponse as PubSubIQ;
                if ((psiq.PubSub != null) && (psiq.PubSub.Items != null))
                {
                    strNodeJID = psiq.PubSub.Items.Node;

                    if (psiq.PubSub.Items.Items != null)
                    {
                        foreach (PubSubItem item in psiq.PubSub.Items.Items)
                        {
                            returnnodes.Add(item);
                        }
                    }
                }
            }

            return returnnodes.ToArray();
        }

        static string GetNodeItemsXML =
  @"<pubsub xmlns='http://jabber.org/protocol/pubsub'> <items node='#NODE#'/> </pubsub>";


        public static string[] GetNodeItemStrings(XMPPClient connection, string strNode, out string strNodeJID)
        {
            strNodeJID = "";
            List<string> returnnodes = new List<string>();
            PubSubIQ IQRequest = new PubSubIQ();
            IQRequest.Type = IQType.get.ToString();
            IQRequest.From = connection.JID;
            IQRequest.To = string.Format("pubsub.{0}", connection.Domain);
            IQRequest.PubSub = new PubSub();
            IQRequest.PubSub.Items = new PubSubItems();
            IQRequest.PubSub.Items.Node = strNode; 
            

            //IQRequest.InnerXML = GetNodeItemsXML.Replace("#NODE#", strNode);;

            IQ IQResponse = connection.SendRecieveIQ(IQRequest, 30000, SerializationMethod.XMLSerializeObject);

            if (IQResponse.Type == IQType.error.ToString())
            {
                return returnnodes.ToArray();
            }

            if (IQResponse is PubSubIQ)
            {
                PubSubIQ psiq = IQResponse as PubSubIQ;
                if ((psiq.PubSub != null) && (psiq.PubSub.Items != null) )
                {
                    strNodeJID = psiq.PubSub.Items.Node;

                    if (psiq.PubSub.Items.Items != null)
                    {
                        foreach (PubSubItem item in psiq.PubSub.Items.Items)
                        {
                            returnnodes.Add(item.InnerItemXML.Value);
                        }
                    }
                }
            }

            //var nodes = IQResponse.InitalXMLElement.Descendants("{http://jabber.org/protocol/pubsub}items");
            //foreach (XElement elem in nodes)
            //{
            //    strNodeJID = elem.Attribute("node").Value;
            //}

            //nodes = IQResponse.InitalXMLElement.Descendants("{http://jabber.org/protocol/pubsub}item");
            //foreach (XElement elem in nodes)
            //{
            //    returnnodes.Add(elem.Value);
            //}
            return returnnodes.ToArray();
        }


        static string DeleteNodeXML =
  @"<pubsub xmlns='http://jabber.org/protocol/pubsub#owner'>
      <delete node='#NODE#' />
 </pubsub>";


        public static bool DeleteNode(XMPPClient connection, string strNode)
        {
            string strXML = DeleteNodeXML.Replace("#NODE#", strNode);
            IQ IQRequest = new IQ();
            IQRequest.Type = IQType.set.ToString();
            IQRequest.From = connection.JID;
            IQRequest.To = string.Format("pubsub.{0}", connection.Domain);
            IQRequest.InnerXML = strXML;

            IQ IQResponse = connection.SendRecieveIQ(IQRequest, 10000);

            if (IQResponse.Type == IQType.error.ToString())
            {
                return false;
            }

            return true;
        }

        public static string RetractNodeXML =
@"<pubsub xmlns='http://jabber.org/protocol/pubsub'>
      <retract node='#NODE#'>
          <item id='#ITEM#' />
       </retract>
 </pubsub>";


        /// <summary>
        /// Retracts an item on the pubsub node.  This client must be the owner
        /// </summary>
        /// <param name="strNode"></param>
        /// <param name="strItem"></param>
        /// <param name="nTimeoutMs"></param>
        /// <returns></returns>
        public static bool RetractItem(XMPPClient connection, string strNode, string strItem)
        {
            IQ pubsub = new IQ();
            pubsub.Type = IQType.set.ToString();
            pubsub.From = connection.JID;
            pubsub.To = new JID(string.Format("pubsub.{0}", connection.Domain));

            string strXML = RetractNodeXML.Replace("#NODE#", strNode);
            strXML = strXML.Replace("#ITEM#", strItem);
            pubsub.InnerXML = strXML;

            IQ IQResponse = connection.SendRecieveIQ(pubsub, 10000);

            if (IQResponse.Type == IQType.error.ToString())
            {
                return false;
            }

            return true;
        }

        public static string PurgeNodeXML =
@"<pubsub xmlns='http://jabber.org/protocol/pubsub'>
      <purge node='#NODE#' />
 </pubsub>";


        /// <summary>
        /// Purges the items in a node
        /// </summary>
        /// <param name="strNode"></param>
        /// <returns></returns>
        public static bool PurgeNode(XMPPClient connection, string strNode, bool bWaitForResponse)
        {
            IQ pubsub = new IQ();
            pubsub.Type = IQType.set.ToString();
            pubsub.From = connection.JID;
            pubsub.To = new JID(string.Format("pubsub.{0}", connection.Domain));

            pubsub.InnerXML = PurgeNodeXML.Replace("#NODE#", strNode);

            if (bWaitForResponse == false)
            {
                connection.SendXMPP(pubsub);
                return true;
            }

            IQ IQResponse = connection.SendRecieveIQ(pubsub, 10000);

            if (IQResponse.Type == IQType.error.ToString())
            {
                return false;
            }

            return true;
        }


        public static string SubscribeNodeXML =
@"<pubsub xmlns='http://jabber.org/protocol/pubsub'>
      <subscribe node='#NODE#' jid='#JID#' />
 </pubsub>";


        /// <summary>
        /// Purges the items in a node
        /// </summary>
        /// <param name="strNode"></param>
        /// <returns></returns>
        public static string SubscribeNode(XMPPClient connection, string strNode, string strJID, bool bWaitForResponse)
        {
            IQ pubsub = new IQ();
            pubsub.Type = IQType.set.ToString();
            pubsub.From = connection.JID;
            pubsub.To = new JID(string.Format("pubsub.{0}", connection.Domain));

            pubsub.InnerXML = SubscribeNodeXML.Replace("#NODE#", strNode).Replace("#JID#", strJID);

            if (bWaitForResponse == false)
            {
                connection.SendXMPP(pubsub);
                return null;
            }

            IQ IQResponse = connection.SendRecieveIQ(pubsub, 20000);

            if ( (IQResponse == null) || (IQResponse.Type == IQType.error.ToString()) )
            {
                return null;
            }
            
            ///<iq type="result" id="9933bb36-7685-460f-8d7f-e9e5ad80f780" from="pubsub.ninethumbs.com" to="test@ninethumbs.com/9047e898-aed2-4f58-b3e2-a5f671758544">
            ///     <pubsub xmlns="http://jabber.org/protocol/pubsub">
            ///         <subscription node="GroceryList" jid="test@ninethumbs.com/9047e898-aed2-4f58-b3e2-a5f671758544" subid="tU98QlKDYuEhTiKO443Vs2AWi2Y07i4Pf2l1wc8W" subscription="subscribed">
            ///         <subscribe-options/></subscription>
            ///     </pubsub>
            /// </iq>
            /// 

            if (IQResponse is PubSubIQ)
            {
                PubSubIQ psiq = IQResponse as PubSubIQ;
                if ( (psiq.PubSub != null) && (psiq.PubSub.Subscription != null) )
                    return psiq.PubSub.Subscription.subid;
            }

            return null;
        }

        public static string UnsubscribeNodeXML =
@"<pubsub xmlns='http://jabber.org/protocol/pubsub'>
      <unsubscribe node='#NODE#' jid='#JID#' sid='#SID#' />
 </pubsub>";


        /// <summary>
        /// Purges the items in a node
        /// </summary>
        /// <param name="strNode"></param>
        /// <returns></returns>
        public static bool UnsubscribeNode(XMPPClient connection, string strNode, string strJID, string strSID, bool bWaitForResponse)
        {
            IQ pubsub = new IQ();
            pubsub.Type = IQType.set.ToString();
            pubsub.From = connection.JID;
            pubsub.To = new JID(string.Format("pubsub.{0}", connection.Domain));

            pubsub.InnerXML = UnsubscribeNodeXML.Replace("#NODE#", strNode).Replace("#JID#", strJID).Replace("#SID#", strSID);

            if (bWaitForResponse == false)
            {
                connection.SendXMPP(pubsub);
                return true;
            }

            IQ IQResponse = connection.SendRecieveIQ(pubsub, 10000);

            if (IQResponse.Type == IQType.error.ToString())
            {
                return false;
            }

            return true;
        }


        static string PublishItemXML =
@"<pubsub xmlns='http://jabber.org/protocol/pubsub'>
      <publish node='#NODE#'>
          <item id='#ITEM#'>
            #ITEMXML#
          </item>
       </publish>
 </pubsub>";

        public static bool PublishItem(XMPPClient connection, string strNode, string strItemName, string strItemXML)
        {
            IQ pubsub = new IQ();
            pubsub.Type = IQType.set.ToString();
            pubsub.From = connection.JID;
            pubsub.To = new JID(string.Format("pubsub.{0}", connection.Domain));


            string strXML = PublishItemXML.Replace("#NODE#", strNode);
            strXML = strXML.Replace("#ITEM#", strItemName);
            strXML = strXML.Replace("#ITEMXML#", strItemXML);
            pubsub.InnerXML = strXML;

            IQ IQResponse = connection.SendRecieveIQ(pubsub, 15000);

            if (IQResponse.Type == IQType.error.ToString())
            {
                return false;
            }
            return true;
        }

        static string RequestItemXML =
@"<pubsub xmlns='http://jabber.org/protocol/pubsub'>
      <items node='#NODE#'>
          <item id='#ITEM#' />
       </items>
 </pubsub>";


        public static IQ RequestItem(XMPPClient connection, string strNode, string strItemName)
        {
            IQ pubsub = new IQ();
            pubsub.Type = IQType.get.ToString();
            pubsub.From = connection.JID;
            pubsub.To = new JID(string.Format("pubsub.{0}", connection.Domain));


            string strXML = RequestItemXML.Replace("#NODE#", strNode);
            strXML = strXML.Replace("#ITEM#", strItemName);
            pubsub.InnerXML = strXML;

            IQ IQResponse = connection.SendRecieveIQ(pubsub, 15000);

            if (IQResponse.Type == IQType.error.ToString())
            {
                return null;
            }
            return IQResponse;
        }

        static string CreateNodeXML =
@"<pubsub xmlns='http://jabber.org/protocol/pubsub'>
      <create node='#NODE#' />
      <configure>
            #x#
       </configure>
 </pubsub>";

        public static void CreateNode(XMPPClient connection, string strNode, string strParentNode, PubSubConfigForm nodeform)
        {
            IQ pubsub = new IQ();
            pubsub.Type = IQType.set.ToString();
            pubsub.From = connection.JID;
            pubsub.To = new JID(string.Format("pubsub.{0}", connection.Domain));


            string strXML = CreateNodeXML.Replace("#NODE#", strNode);

            /// Get inner xml
            /// 
            string strForm = nodeform.BuildAskingForm(nodeform);

            strXML = strXML.Replace("#x#", strForm);
            pubsub.InnerXML = strXML;

            IQ IQResponse = connection.SendRecieveIQ(pubsub, 10000);
            if (IQResponse == null)
                return;
            if (IQResponse.Type == IQType.error.ToString())
            {
                return;
            }
        }

    }

}
