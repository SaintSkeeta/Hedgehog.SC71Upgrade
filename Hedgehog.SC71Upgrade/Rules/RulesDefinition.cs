using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Extensions.StringExtensions;
using Sitecore.Extensions.XElementExtensions;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Hedgehog.SC71Upgrade.Rules
{
    /// <summary>
    /// Defines the RulesDefinition type.
    /// </summary>
    public class RulesDefinition
    {
        /// <summary>
        /// Regular expression for macros search.
        /// </summary>
        private const string MacrosRegex = "\\[([^\\]])*\\]";
        /// <summary>
        /// Rules definition document.
        /// </summary>
        private readonly XDocument document;
        /// <summary>
        /// Gets the rules definition document.
        /// </summary>
        public XDocument Document
        {
            get
            {
                return this.document;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Rules.RulesDefinition" /> class.
        /// </summary>
        /// <param name="rulesXml">The rules XML.</param>
        public RulesDefinition(string rulesXml)
        {
            this.document = (string.IsNullOrEmpty(rulesXml) ? XDocument.Parse("<ruleset />") : XDocument.Parse(rulesXml));
        }
        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>Returns the attribute.</returns>
        public static string GetAttribute(XElement element, string attributeName)
        {
            Assert.ArgumentNotNull(element, "element");
            XAttribute xAttribute = element.Attribute(attributeName);
            if (xAttribute == null)
            {
                return null;
            }
            return xAttribute.Value;
        }
        /// <summary>
        /// Gets the rule title.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="index">The index.</param>
        /// <returns>Returns the rule title.</returns>
        public static string GetRuleTitle(XElement rule, int index)
        {
            Assert.ArgumentNotNull(rule, "rule");
            string attributeValue = rule.GetAttributeValue("name");
            if (!string.IsNullOrEmpty(attributeValue))
            {
                return attributeValue;
            }
            return Translate.Text("Rule") + " " + index;
        }
        /// <summary>
        /// Sets the attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="value">The value.</param>
        public static void SetAttribute(XElement element, string attributeName, string value)
        {
            Assert.ArgumentNotNull(element, "element");
            XAttribute xAttribute = element.Attribute(attributeName);
            if (xAttribute == null)
            {
                xAttribute = new XAttribute(attributeName, value);
                element.Add(xAttribute);
                return;
            }
            xAttribute.Value = value;
        }
        /// <summary>
        /// Applies the unique ids to rules.
        /// </summary>
        public void ApplyUniqueIdsToRules()
        {
            foreach (XElement current in this.document.XPathSelectElements("//rule"))
            {
                string attributeValue = current.GetAttributeValue("uid");
                if (string.IsNullOrEmpty(attributeValue))
                {
                    current.SetAttributeValue("uid", ID.NewID.ToString());
                }
            }
        }
        /// <summary>
        /// Adds the action.
        /// </summary>
        /// <returns>Returns the rule.</returns>
        public XElement AddRule()
        {
            return this.AddRule(System.Guid.NewGuid());
        }
        /// <summary>
        /// Adds the rule.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <returns>Returns the rule.</returns>
        public XElement AddRule(System.Guid ruleId)
        {
            return this.GetRule(ruleId, true);
        }
        /// <summary>
        /// Adds the action.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="actionId">The action id.</param>
        /// <returns>
        /// Returns the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">actionId is not a valid <c>Guid</c>.</exception>
        public XElement AddAction(System.Guid ruleId, string actionId)
        {
            Assert.ArgumentNotNullOrEmpty(actionId, "actionId");
            System.Guid actionId2;
            if (!System.Guid.TryParse(actionId, out actionId2))
            {
                throw new System.ArgumentException("actionId is not a valid Guid.", "actionId");
            }
            return this.AddAction(ruleId, actionId2);
        }
        /// <summary>
        /// Adds the action.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="actionId">The action id.</param>
        /// <returns>
        /// Returns the action.
        /// </returns>
        public XElement AddAction(System.Guid ruleId, System.Guid actionId)
        {
            Assert.ArgumentNotNull(actionId, "actionId");
            return this.AddAction(ruleId, new ID(actionId));
        }
        /// <summary>
        /// Adds the action.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="actionId">The action id.</param>
        /// <returns>Returns the action.</returns>
        public XElement AddAction(System.Guid ruleId, ID actionId)
        {
            Assert.ArgumentNotNull(actionId, "actionId");
            return this.AddAction(this.GetRule(ruleId), actionId);
        }
        /// <summary>
        /// Adds the condition.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="conditionId">The condition id.</param>
        /// <returns>
        /// Returns the condition.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">conditionId is not a valid <c>Guid</c>.</exception>
        public XElement AddCondition(System.Guid ruleId, string conditionId)
        {
            Assert.ArgumentNotNullOrEmpty(conditionId, "conditionId");
            System.Guid conditionId2;
            if (!System.Guid.TryParse(conditionId, out conditionId2))
            {
                throw new System.ArgumentException("conditionId is not a valid Guid.", "conditionId");
            }
            return this.AddCondition(ruleId, conditionId2);
        }
        /// <summary>
        /// Adds the condition.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="conditionId">The condition id.</param>
        /// <returns>Returns the condition.</returns>
        public XElement AddCondition(System.Guid ruleId, System.Guid conditionId)
        {
            Assert.ArgumentNotNull(conditionId, "conditionId");
            return this.AddCondition(ruleId, new ID(conditionId));
        }
        /// <summary>
        /// Adds the condition.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="conditionId">The condition id.</param>
        /// <returns>Returns the condition.</returns>
        public XElement AddCondition(System.Guid ruleId, ID conditionId)
        {
            Assert.ArgumentNotNull(conditionId, "conditionId");
            return this.AddCondition(this.GetRule(ruleId), conditionId);
        }
        /// <summary>
        /// Gets the element by unique id.
        /// </summary>
        /// <param name="uniqueId">The unique id.</param>
        /// <returns>Returns the element by unique id.</returns>
        public XElement GetElementByUniqueId(string uniqueId)
        {
            Assert.ArgumentNotNull(uniqueId, "uniqueId");
            return this.document.XPathSelectElement("//*[@uid='" + uniqueId + "']");
        }
        /// <summary>
        /// Gets the id by unique id.
        /// </summary>
        /// <param name="uniqueId">The unique id.</param>
        /// <returns>The id.</returns>
        public string GetIdByUniqueId(string uniqueId)
        {
            XElement elementByUniqueId = this.GetElementByUniqueId(uniqueId);
            if (elementByUniqueId == null)
            {
                return null;
            }
            return elementByUniqueId.GetAttributeValue("id");
        }
        /// <summary>
        /// Gets the unique id.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The unique id.</returns>
        public string GetUniqueId(XElement element)
        {
            Assert.ArgumentNotNull(element, "element");
            return element.GetAttributeValue("uid");
        }
        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="actionId">The action id.</param>
        /// <returns>The action.</returns>
        public XElement GetAction(System.Guid ruleId, ID actionId)
        {
            XElement rule = this.GetRule(ruleId);
            if (rule == null)
            {
                return null;
            }
            return rule.Descendants("action").FirstOrDefault((XElement a) => actionId.ToString().Equals(a.GetAttributeValue("id"), System.StringComparison.InvariantCultureIgnoreCase));
        }
        /// <summary>
        /// Gets the condition.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="conditionId">The condition id.</param>
        /// <returns>The condition.</returns>
        public XElement GetCondition(System.Guid ruleId, ID conditionId)
        {
            XElement rule = this.GetRule(ruleId);
            if (rule == null)
            {
                return null;
            }
            return rule.Descendants("condition").FirstOrDefault((XElement a) => conditionId.ToString().Equals(a.GetAttributeValue("id"), System.StringComparison.InvariantCultureIgnoreCase));
        }
        /// <summary>
        /// Gets the conditions.
        /// </summary>
        /// <param name="conditionId">The condition id.</param>
        /// <returns>The conditions.</returns>
        public System.Collections.Generic.IEnumerable<XElement> GetConditions(ID conditionId)
        {
            return
                from a in this.document.Descendants("condition")
                where conditionId.ToString().Equals(a.GetAttributeValue("id"), System.StringComparison.InvariantCultureIgnoreCase)
                select a;
        }
        /// <summary>
        /// Gets the rule.
        /// </summary>
        /// <param name="ruleId">The current rule id.</param>
        /// <returns>Returns the rule.</returns>
        public XElement GetRule(System.Guid ruleId)
        {
            return this.GetRule(ruleId, false);
        }
        /// <summary>
        /// Gets the rule containing element.
        /// </summary>
        /// <param name="containedElement">The contained element.</param>
        /// <returns>The rule.</returns>
        public XElement GetRuleContainingElement(XElement containedElement)
        {
            Assert.ArgumentNotNull(containedElement, "containedElement");
            return containedElement.Ancestors("rule").FirstOrDefault<XElement>();
        }
        /// <summary>
        /// Gets the rules.
        /// </summary>
        /// <returns>Returns the collection of rules.</returns>
        public System.Collections.Generic.IEnumerable<XElement> GetRules()
        {
            XElement root = this.document.Root;
            if (root == null)
            {
                return null;
            }
            return root.XPathSelectElements("//rule");
        }
        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        /// <returns>The validation errors.</returns>
        public System.Collections.Generic.List<string> GetValidationErrors()
        {
            System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
            System.Collections.Generic.IEnumerable<XElement> enumerable = this.Document.Descendants("rule");
            foreach (XElement current in enumerable)
            {
                System.Collections.Generic.IEnumerable<XElement> enumerable2 = current.Descendants("action");
                foreach (XElement current2 in enumerable2)
                {
                    this.GetValidationErrorsForActionOrCondition(current, current2, list);
                }
                System.Collections.Generic.IEnumerable<XElement> enumerable3 = current.Descendants("condition");
                foreach (XElement current3 in enumerable3)
                {
                    this.GetValidationErrorsForActionOrCondition(current, current3, list);
                }
            }
            return list;
        }
        /// <summary>
        /// Determines whether action with the specified id is referenced.
        /// </summary>
        /// <param name="actionId">The action id.</param>
        /// <returns>
        ///   <c>true</c> if action with the specified id is referenced; otherwise, <c>false</c>.
        /// </returns>
        public bool IsActionReferenced(ID actionId)
        {
            Assert.ArgumentNotNull(actionId, "actionId");
            return this.Document.Descendants("action").Any((XElement a) => actionId.ToString().Equals(a.GetAttributeValue("id"), System.StringComparison.InvariantCultureIgnoreCase));
        }
        /// <summary>
        /// Determines whether condition with the specified id is referenced.
        /// </summary>
        /// <param name="conditionId">The condition id.</param>
        /// <returns>
        ///   <c>true</c> if condition with the specified id is referenced; otherwise, <c>false</c>.
        /// </returns>
        public bool IsConditionReferenced(ID conditionId)
        {
            return this.IsConditionReferenced(null, conditionId);
        }
        /// <summary>
        /// Determines whether condition with the specified id is referenced.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="conditionId">The condition id.</param>
        /// <returns>
        ///   <c>true</c> if condition with the specified id is referenced; otherwise, <c>false</c>.
        /// </returns>
        public bool IsConditionReferenced(System.Guid ruleId, ID conditionId)
        {
            return this.IsConditionReferenced(new System.Guid?(ruleId), conditionId);
        }
        /// <summary>
        /// Determines whether item with the specified id is referenced by any action or condition.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <returns>
        ///   <c>true</c> if item with the specified id is referenced by any action or condition; otherwise, <c>false</c>.
        /// </returns>
        public bool IsItemReferenced(ID itemId)
        {
            Assert.ArgumentNotNull(itemId, "itemId");
            return this.GetAttributesReferencingItem(itemId).Count > 0;
        }
        /// <summary>
        /// Moves the specified id.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="uniqueId">The id.</param>
        /// <param name="direction">The direction.</param>
        public void Move(System.Guid ruleId, string uniqueId, string direction)
        {
            Assert.ArgumentNotNull(ruleId, "ruleId");
            Assert.ArgumentNotNull(uniqueId, "id");
            Assert.ArgumentNotNull(direction, "direction");
            XElement rule = this.GetRule(ruleId);
            if (rule == null)
            {
                return;
            }
            XElement xElement = rule.Element("conditions");
            if (xElement == null)
            {
                return;
            }
            System.Collections.Generic.List<XElement> list = new System.Collections.Generic.List<XElement>();
            RulesDefinition.Flatten(xElement, list, "or");
            XElement element = RulesDefinition.GetElement(list, uniqueId);
            int num = list.IndexOf(element);
            if (direction == "up")
            {
                if (num > 0)
                {
                    list.RemoveAt(num);
                    list.Insert(num - 1, element);
                }
            }
            else
            {
                if (num < list.Count - 1)
                {
                    list.RemoveAt(num);
                    list.Insert(num + 1, element);
                }
            }
            RulesDefinition.Deepen(xElement, list);
        }
        /// <summary>
        /// Removes the action.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="actionUniqueId">The action unique id.</param>
        public void RemoveAction(System.Guid ruleId, string actionUniqueId)
        {
            Assert.ArgumentNotNull(ruleId, "ruleId");
            Assert.ArgumentNotNull(actionUniqueId, "actionUniqueId");
            XElement rule = this.GetRule(ruleId);
            if (rule == null)
            {
                return;
            }
            this.RemoveAction(rule, actionUniqueId);
        }
        /// <summary>Removes the references to the action.</summary>
        /// <param name="actionId">The action id.</param>
        public void RemoveActionReferences(ID actionId)
        {
            Assert.ArgumentNotNull(actionId, "actionId");
            System.Collections.Generic.List<XElement> rulesByActionId = this.GetRulesByActionId(actionId);
            if (rulesByActionId == null)
            {
                return;
            }
            foreach (XElement current in rulesByActionId)
            {
                this.RemoveAction(current, actionId.ToString());
            }
        }
        /// <summary>
        /// Removes the condition.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="conditionUniqueId">The condition unique id.</param>
        public void RemoveCondition(System.Guid ruleId, string conditionUniqueId)
        {
            Assert.ArgumentNotNull(ruleId, "ruleId");
            Assert.ArgumentNotNull(conditionUniqueId, "conditionUniqueId");
            XElement rule = this.GetRule(ruleId);
            if (rule == null)
            {
                return;
            }
            this.RemoveCondition(rule, conditionUniqueId);
        }
        /// <summary>Removes the references to the condition.</summary>
        /// <param name="conditionId">The condition id.</param>
        public void RemoveConditionReferences(ID conditionId)
        {
            Assert.ArgumentNotNull(conditionId, "conditionId");
            System.Collections.Generic.List<XElement> rulesByConditionId = this.GetRulesByConditionId(conditionId);
            if (rulesByConditionId == null)
            {
                return;
            }
            foreach (XElement current in rulesByConditionId)
            {
                this.RemoveCondition(current, conditionId.ToString());
            }
        }
        /// <summary>
        /// Removes the references to the item.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        public void RemoveItemReferences(ID itemId)
        {
            Assert.ArgumentNotNull(itemId, "itemId");
            System.Collections.Generic.List<XAttribute> attributesReferencingItem = this.GetAttributesReferencingItem(itemId);
            foreach (XAttribute current in attributesReferencingItem)
            {
                current.Remove();
            }
        }
        /// <summary>
        /// Removes the rule.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="nextRuleId">The next rule id.</param>
        /// <returns>Boolean value indicating whether rule has been removed.</returns>
        public bool RemoveRule(System.Guid ruleId, out System.Guid nextRuleId)
        {
            nextRuleId = System.Guid.Empty;
            XElement rule = this.GetRule(ruleId);
            if (rule == null)
            {
                return false;
            }
            XElement xElement = (rule.NextNode as XElement) ?? (rule.PreviousNode as XElement);
            nextRuleId = ((xElement != null) ? new System.Guid(xElement.GetAttributeValue("uid")) : System.Guid.Empty);
            rule.Remove();
            return true;
        }
        /// <summary>
        /// Replaces the action references.
        /// </summary>
        /// <param name="oldActionId">The old action id.</param>
        /// <param name="newActionId">The new action id.</param>
        public void ReplaceActionReferences(ID oldActionId, ID newActionId)
        {
            Assert.ArgumentNotNull(oldActionId, "oldActionId");
            Assert.ArgumentNotNull(newActionId, "newActionId");
            this.ReplaceElementReferences(this.Document.Descendants("action"), oldActionId, newActionId);
        }
        /// <summary>
        /// Replaces the condition references.
        /// </summary>
        /// <param name="oldConditionId">The old condition id.</param>
        /// <param name="newConditionId">The new condition id.</param>
        public void ReplaceConditionReferences(ID oldConditionId, ID newConditionId)
        {
            Assert.ArgumentNotNull(oldConditionId, "oldConditionId");
            Assert.ArgumentNotNull(newConditionId, "newConditionId");
            this.ReplaceElementReferences(this.Document.Descendants("condition"), oldConditionId, newConditionId);
        }
        /// <summary>
        /// Replaces the item references.
        /// </summary>
        /// <param name="oldItemId">The old item id.</param>
        /// <param name="newItemId">The new item id.</param>
        public void ReplaceItemReferences(ID oldItemId, ID newItemId)
        {
            Assert.ArgumentNotNull(oldItemId, "oldItemId");
            Assert.ArgumentNotNull(newItemId, "newItemId");
            System.Collections.Generic.IEnumerable<XAttribute> enumerable = this.Document.Descendants("action").Attributes();
            enumerable = enumerable.Union(this.Document.Descendants("condition").Attributes());
            foreach (XAttribute current in enumerable)
            {
                ID id;
                if (!string.Equals("id", current.Name.LocalName, System.StringComparison.InvariantCultureIgnoreCase) && !string.Equals("uid", current.Name.LocalName, System.StringComparison.InvariantCultureIgnoreCase) && ID.TryParse(current.Value, out id) && id == oldItemId)
                {
                    current.Value = newItemId.ToString();
                }
            }
        }
        /// <summary>
        /// Sorts the specified sequence.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="sequence">The sequence.</param>
        public void Sort(System.Guid ruleId, string sequence)
        {
            Assert.ArgumentNotNull(sequence, "sequence");
            XElement rule = this.GetRule(ruleId);
            if (rule == null)
            {
                return;
            }
            XElement xElement = rule.Element("conditions");
            if (xElement == null)
            {
                return;
            }
            System.Collections.Generic.List<XElement> elements = new System.Collections.Generic.List<XElement>();
            RulesDefinition.Flatten(xElement, elements, "or");
            System.Collections.Generic.List<XElement> list = new System.Collections.Generic.List<XElement>();
            string[] array = sequence.Split(new char[]
		{
			','
		});
            for (int i = 0; i < array.Length; i++)
            {
                string uniqueId = array[i];
                XElement element = RulesDefinition.GetElement(elements, uniqueId);
                if (element != null)
                {
                    list.Add(element);
                }
            }
            RulesDefinition.Deepen(xElement, list);
        }
        /// <summary>
        /// Toggles the operator.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="uniqueId">The unique id.</param>
        public void ToggleOperator(System.Guid ruleId, string uniqueId)
        {
            Assert.ArgumentNotNull(uniqueId, "uniqueId");
            XElement rule = this.GetRule(ruleId);
            if (rule == null)
            {
                return;
            }
            XElement orCreateChildElement = this.GetOrCreateChildElement(rule, "conditions");
            XElement elementByUniqueId = this.GetElementByUniqueId(uniqueId);
            Assert.IsNotNull(elementByUniqueId, "element is null");
            string text = elementByUniqueId.Name.LocalName;
            string a;
            if ((a = text) != null)
            {
                if (!(a == "and"))
                {
                    if (a == "or")
                    {
                        text = "and";
                    }
                }
                else
                {
                    text = "or";
                }
            }
            elementByUniqueId.Name = text;
            System.Collections.Generic.List<XElement> elements = new System.Collections.Generic.List<XElement>();
            RulesDefinition.Flatten(orCreateChildElement, elements, "or");
            RulesDefinition.Deepen(orCreateChildElement, elements);
        }
        /// <summary>
        /// Toggles the prefix.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="uniqueId">The unique id.</param>
        public void TogglePrefix(System.Guid ruleId, string uniqueId)
        {
            Assert.ArgumentNotNull(uniqueId, "uniqueId");
            XElement rule = this.GetRule(ruleId);
            if (rule == null)
            {
                return;
            }
            this.GetOrCreateChildElement(rule, "conditions");
            XElement elementByUniqueId = this.GetElementByUniqueId(uniqueId);
            Assert.IsNotNull(elementByUniqueId, "element is null");
            bool flag = elementByUniqueId.GetAttributeValue("except") == "true";
            elementByUniqueId.SetAttributeValue("except", flag ? null : "true");
        }
        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Document.ToString();
        }
        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public string ToString(SaveOptions options)
        {
            return this.Document.ToString(options);
        }
        /// <summary>
        /// Gets the referenced actions.
        /// </summary>
        /// <returns>List of referenced actions.</returns>
        public System.Collections.Generic.List<ID> GetReferencedActions()
        {
            return (
                from a in this.Document.Descendants("action")
                select a.GetAttributeValue("id")).Where(new Func<string, bool>(ID.IsID)).Select(new Func<string, ID>(ID.Parse)).Distinct<ID>().ToList<ID>();
        }
        /// <summary>
        /// Gets the referenced conditions.
        /// </summary>
        /// <returns>List of referenced conditions.</returns>
        public System.Collections.Generic.List<ID> GetReferencedConditions()
        {
            return (
                from a in this.Document.Descendants("condition")
                select a.GetAttributeValue("id")).Where(new Func<string, bool>(ID.IsID)).Select(new Func<string, ID>(ID.Parse)).Distinct<ID>().ToList<ID>();
        }
        /// <summary>
        /// Gets the items referenced by any action or condition.
        /// </summary>
        /// <returns>List of items referenced by any action or condition.</returns>
        public System.Collections.Generic.List<ID> GetReferencedItems()
        {
            System.Collections.Generic.List<ID> list = new System.Collections.Generic.List<ID>();
            System.Collections.Generic.IEnumerable<XAttribute> enumerable = this.Document.Descendants("action").Attributes();
            enumerable = enumerable.Union(this.Document.Descendants("condition").Attributes());
            foreach (XAttribute current in enumerable)
            {
                ID item;
                if (!string.Equals("id", current.Name.LocalName, System.StringComparison.InvariantCultureIgnoreCase) && !string.Equals("uid", current.Name.LocalName, System.StringComparison.InvariantCultureIgnoreCase) && ID.TryParse(current.Value, out item) && !list.Contains(item))
                {
                    list.Add(item);
                }
            }
            return list;
        }
        /// <summary>
        /// Adds the new condition.
        /// </summary>
        /// <param name="conditionsElement">The conditions element.</param>
        /// <param name="conditionId">The condition id.</param>
        /// <returns>Returns the condition.</returns>
        private static XElement AddNewConditionElement(XElement conditionsElement, ID conditionId)
        {
            Assert.ArgumentNotNull(conditionsElement, "conditionsElement");
            Assert.ArgumentNotNull(conditionId, "conditionId");
            XElement xElement = new XElement("condition");
            xElement.Add(new XAttribute("id", conditionId.ToString()));
            xElement.Add(new XAttribute("uid", ID.NewID.ToShortID().ToString()));
            XElement xElement2 = conditionsElement.Element(0);
            if (xElement2 == null)
            {
                conditionsElement.Add(xElement);
                return xElement;
            }
            XElement rightLeaf = RulesDefinition.GetRightLeaf(xElement2);
            XElement xElement3 = new XElement("and");
            xElement3.Add(new XAttribute("uid", ID.NewID.ToShortID().ToString()));
            rightLeaf.ReplaceWith(xElement3);
            xElement3.Add(rightLeaf);
            xElement3.Add(xElement);
            return xElement;
        }
        /// <summary>Flattens the specified conditions.</summary>
        /// <param name="parent">The conditions.</param>
        /// <param name="elements">The elements.</param>
        /// <param name="op">The operator.</param>
        private static void Flatten(XElement parent, System.Collections.Generic.List<XElement> elements, string op)
        {
            Assert.ArgumentNotNull(parent, "parent");
            Assert.ArgumentNotNull(elements, "elements");
            Assert.ArgumentNotNull(op, "op");
            if (parent.Name.LocalName == "condition")
            {
                elements.Add(parent);
                parent.SetAttributeValue("_operator", op);
                return;
            }
            if (parent.Name.LocalName == "not")
            {
                XElement xElement = parent.Element(0);
                if (xElement == null)
                {
                    return;
                }
                RulesDefinition.Flatten(xElement, elements, op + "not");
                return;
            }
            else
            {
                XElement xElement2 = parent.Element(0);
                if (xElement2 == null)
                {
                    return;
                }
                RulesDefinition.Flatten(xElement2, elements, op);
                XElement xElement3 = parent.Element(1);
                if (xElement3 == null)
                {
                    return;
                }
                RulesDefinition.Flatten(xElement3, elements, parent.Name.LocalName);
                return;
            }
        }
        /// <summary>Deepens the specified elements.</summary>
        /// <param name="conditionsElement">The conditions element.</param>
        /// <param name="elements">The elements.</param>
        private static void Deepen(XElement conditionsElement, System.Collections.Generic.List<XElement> elements)
        {
            Assert.ArgumentNotNull(conditionsElement, "conditionsElement");
            Assert.ArgumentNotNull(elements, "elements");
            conditionsElement.RemoveNodes();
            if (elements.Count == 0)
            {
                return;
            }
            XElement xElement = elements[0];
            if (xElement == null)
            {
                return;
            }
            xElement.SetAttributeValue("_operator", null);
            XElement xElement2 = xElement;
            for (int i = 1; i < elements.Count; i++)
            {
                XElement xElement3 = elements[i];
                string attributeValue = xElement3.GetAttributeValue("_operator");
                xElement3.SetAttributeValue("_operator", null);
                XElement xElement4 = new XElement(attributeValue);
                xElement4.SetAttributeValue("uid", ID.NewID.ToShortID().ToString());
                if (attributeValue == "and")
                {
                    Assert.IsNotNull(xElement, "current  is null");
                    XElement parent = xElement.Parent;
                    if (parent != null)
                    {
                        xElement.Remove();
                        parent.Add(xElement4);
                    }
                    xElement4.Add(xElement);
                    xElement4.Add(xElement3);
                    if (xElement == xElement2)
                    {
                        xElement2 = xElement4;
                    }
                    xElement = xElement4.Element(1);
                }
                else
                {
                    xElement4.Add(xElement2);
                    xElement4.Add(xElement3);
                    xElement = xElement4.Element(1);
                    xElement2 = xElement4;
                }
            }
            conditionsElement.Add(xElement2);
        }
        /// <summary>Gets the right leaf.</summary>
        /// <param name="element">The element.</param>
        /// <returns>Returns the right leaf element.</returns>
        private static XElement GetRightLeaf(XElement element)
        {
            Assert.ArgumentNotNull(element, "element");
            XElement xElement = element.Element(1);
            if (xElement != null)
            {
                return RulesDefinition.GetRightLeaf(xElement);
            }
            return element;
        }
        /// <summary>Gets the element.</summary>
        /// <param name="elements">The elements.</param>
        /// <param name="uniqueId">The element unique id.</param>
        /// <returns>The element.</returns>
        private static XElement GetElement(System.Collections.Generic.IEnumerable<XElement> elements, string uniqueId)
        {
            Assert.ArgumentNotNull(elements, "elements");
            Assert.ArgumentNotNull(uniqueId, "uniqueId");
            foreach (XElement current in elements)
            {
                string attributeValue = current.GetAttributeValue("uid");
                if (attributeValue == uniqueId)
                {
                    return current;
                }
            }
            return null;
        }
        /// <summary>
        /// Adds the action.
        /// </summary>
        /// <param name="ruleElement">The rule element.</param>
        /// <param name="actionId">The action id.</param>
        /// <returns>Returns the action.</returns>
        private XElement AddAction(XElement ruleElement, ID actionId)
        {
            Assert.ArgumentNotNull(actionId, "actionId");
            if (ruleElement == null)
            {
                return null;
            }
            XElement orCreateChildElement = this.GetOrCreateChildElement(ruleElement, "actions");
            XElement xElement = new XElement("action");
            xElement.Add(new XAttribute("id", actionId.ToString()));
            xElement.Add(new XAttribute("uid", ID.NewID.ToShortID().ToString()));
            orCreateChildElement.Add(xElement);
            return xElement;
        }
        /// <summary>
        /// Adds the condition.
        /// </summary>
        /// <param name="ruleElement">The rule element.</param>
        /// <param name="conditionId">The condition id.</param>
        /// <returns>Returns the condition.</returns>
        private XElement AddCondition(XElement ruleElement, ID conditionId)
        {
            Assert.ArgumentNotNull(conditionId, "conditionId");
            if (ruleElement == null)
            {
                return null;
            }
            XElement orCreateChildElement = this.GetOrCreateChildElement(ruleElement, "conditions");
            return RulesDefinition.AddNewConditionElement(orCreateChildElement, conditionId);
        }
        /// <summary>
        /// Gets the rules that contain specified condition.
        /// </summary>
        /// <param name="conditionId">The condition id.</param>
        /// <returns>
        /// The rule elements.
        /// </returns>
        private System.Collections.Generic.List<XElement> GetRulesByConditionId(ID conditionId)
        {
            Assert.ArgumentNotNull(conditionId, "conditionId");
            return (
                from r in this.Document.Descendants("rule")
                where r.Descendants("condition").Any((XElement c) => c.Attribute("id") != null && c.Attribute("id").Value == conditionId.ToString())
                select r).ToList<XElement>();
        }
        /// <summary>
        /// Gets the rules that contain specified action.
        /// </summary>
        /// <param name="actionId">The action id.</param>
        /// <returns>
        /// The rule elements.
        /// </returns>
        private System.Collections.Generic.List<XElement> GetRulesByActionId(ID actionId)
        {
            Assert.ArgumentNotNull(actionId, "actionId");
            return (
                from r in this.Document.Descendants("rule")
                where r.Descendants("action").Any((XElement a) => a.Attribute("id") != null && a.Attribute("id").Value == actionId.ToString())
                select r).ToList<XElement>();
        }
        /// <summary>
        /// Gets the attributes that references the item.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <returns>The attributes that references the item.</returns>
        private System.Collections.Generic.List<XAttribute> GetAttributesReferencingItem(ID itemId)
        {
            Assert.ArgumentNotNull(itemId, "itemId");
            System.Collections.Generic.List<XAttribute> list = new System.Collections.Generic.List<XAttribute>();
            System.Collections.Generic.IEnumerable<XAttribute> enumerable = this.Document.Descendants("action").Attributes();
            enumerable = enumerable.Union(this.Document.Descendants("condition").Attributes());
            foreach (XAttribute current in enumerable)
            {
                ID id;
                if (!string.Equals("id", current.Name.LocalName, System.StringComparison.InvariantCultureIgnoreCase) && !string.Equals("uid", current.Name.LocalName, System.StringComparison.InvariantCultureIgnoreCase) && ID.TryParse(current.Value, out id) && id == itemId)
                {
                    list.Add(current);
                }
            }
            return list;
        }
        /// <summary>
        /// Gets or creates the conditions element.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns>
        /// The conditions element.
        /// </returns>
        private XElement GetOrCreateChildElement(XElement parentElement, string elementName)
        {
            Assert.ArgumentNotNull(parentElement, "parentElement");
            Assert.ArgumentNotNullOrEmpty(elementName, "elementName");
            XElement xElement = parentElement.Element(elementName);
            if (xElement == null)
            {
                xElement = new XElement(elementName);
                parentElement.Add(xElement);
            }
            return xElement;
        }
        /// <summary>
        /// Gets the rule.
        /// </summary>
        /// <param name="ruleId">The current rule id.</param>
        /// <param name="create">if set to <c>true</c> rule is created if it does not exist.</param>
        /// <returns>Returns the rule.</returns>
        private XElement GetRule(System.Guid ruleId, bool create)
        {
            XElement root = this.document.Root;
            if (root == null)
            {
                return null;
            }
            string text = ruleId.ToString("B").ToUpperInvariant();
            XElement xElement = root.XPathSelectElement("//rule[@uid='" + text + "']");
            if (xElement == null && create)
            {
                xElement = new XElement("rule");
                xElement.SetAttributeValue("uid", text);
                root.Add(xElement);
            }
            return xElement;
        }
        /// <summary>
        /// Gets the index of the rule (starting with 1).
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns>The index of the rule (starting with 1).</returns>
        private int GetRuleIndex(XElement rule)
        {
            Assert.ArgumentNotNull(rule, "rule");
            if (rule.Document == null)
            {
                return -1;
            }
            return rule.Document.Descendants("rule").ToList<XElement>().IndexOf(rule) + 1;
        }
        /// <summary>
        /// Gets the validation errors for action or condition.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="element">The element.</param>
        /// /// <param name="errors">The errors.</param>
        private void GetValidationErrorsForActionOrCondition(XElement rule, XElement element, System.Collections.Generic.List<string> errors)
        {
            ID itemId;
            if (ID.TryParse(element.GetAttributeValue("id"), out itemId))
            {
                Item item = Sitecore.Context.ContentDatabase.GetItem(itemId);
                if (item != null)
                {
                    string text = item["Text"];
                    string text2 = System.Text.RegularExpressions.Regex.Replace(text, "\\[([^\\]])*\\]", (System.Text.RegularExpressions.Match match) => match.Value.Mid(1, match.Value.Length - 2).Split(new char[]
				{
					','
				})[3]);
                    if (!string.IsNullOrEmpty(text))
                    {
                        System.Text.RegularExpressions.MatchCollection matchCollection = System.Text.RegularExpressions.Regex.Matches(text, "\\[([^\\]])*\\]");
                        foreach (System.Text.RegularExpressions.Match match2 in matchCollection)
                        {
                            string expandedName = match2.Value.Mid(1, match2.Value.Length - 2).Split(new char[]
						{
							','
						})[0];
                            if (element.Attribute(expandedName) == null)
                            {
                                string ruleTitle = RulesDefinition.GetRuleTitle(rule, this.GetRuleIndex(rule));
                                string item2;
                                if (element.Name == "action")
                                {
                                    item2 = Translate.Text("You must enter the required values in the \"{0}\" rule in the \"{1}\" action.", new object[]
								{
									ruleTitle,
									text2
								});
                                }
                                else
                                {
                                    item2 = Translate.Text("You must enter the required values in the \"{0}\" rule in the \"{1}\" condition.", new object[]
								{
									ruleTitle,
									text2
								});
                                }
                                if (!errors.Contains(item2))
                                {
                                    errors.Add(item2);
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Determines whether condition with the specified id is referenced.
        /// </summary>
        /// <param name="ruleId">The rule id.</param>
        /// <param name="conditionId">The condition id.</param>
        /// <returns>
        ///   <c>true</c> if condition with the specified id is referenced; otherwise, <c>false</c>.
        /// </returns>
        private bool IsConditionReferenced(System.Guid? ruleId, ID conditionId)
        {
            Assert.ArgumentNotNull(conditionId, "conditionId");
            XContainer rule = this.Document;
            if (ruleId.HasValue && ruleId != System.Guid.Empty)
            {
                rule = this.GetRule(ruleId.Value);
                Assert.IsNotNull(rule, "rule cannot be null");
            }
            return rule.Descendants("condition").Any((XElement a) => conditionId.ToString().Equals(a.GetAttributeValue("id"), System.StringComparison.InvariantCultureIgnoreCase));
        }
        /// <summary>
        /// Removes the action.
        /// </summary>
        /// <param name="ruleElement">The rule.</param>
        /// <param name="actionIdOrUniqueId">The action id or unique id.</param>
        private void RemoveAction(XElement ruleElement, string actionIdOrUniqueId)
        {
            Assert.ArgumentNotNull(ruleElement, "ruleElement");
            Assert.ArgumentNotNull(actionIdOrUniqueId, "actionIdOrUniqueId");
            XElement xElement = ruleElement.Element("actions");
            if (xElement == null)
            {
                return;
            }
            System.Collections.Generic.List<XElement> list = new System.Collections.Generic.List<XElement>(xElement.Elements());
            foreach (XElement current in list)
            {
                if (actionIdOrUniqueId.Equals(current.GetAttributeValue("uid"), System.StringComparison.InvariantCultureIgnoreCase) || actionIdOrUniqueId.Equals(current.GetAttributeValue("id"), System.StringComparison.InvariantCultureIgnoreCase))
                {
                    current.Remove();
                }
            }
        }
        /// <summary>
        /// Removes the condition.
        /// </summary>
        /// <param name="ruleElement">The rule element.</param>
        /// <param name="conditionIdOrUniqueId">The condition id or unique id.</param>
        private void RemoveCondition(XElement ruleElement, string conditionIdOrUniqueId)
        {
            Assert.ArgumentNotNull(ruleElement, "ruleElement");
            Assert.ArgumentNotNull(conditionIdOrUniqueId, "conditionIdOrUniqueId");
            XElement orCreateChildElement = this.GetOrCreateChildElement(ruleElement, "conditions");
            System.Collections.Generic.List<XElement> list = new System.Collections.Generic.List<XElement>();
            RulesDefinition.Flatten(orCreateChildElement, list, "or");
            for (int i = list.Count - 1; i >= 0; i--)
            {
                XElement xElement = list[i];
                if (conditionIdOrUniqueId.Equals(xElement.GetAttributeValue("uid"), System.StringComparison.InvariantCultureIgnoreCase) || conditionIdOrUniqueId.Equals(xElement.GetAttributeValue("id"), System.StringComparison.InvariantCultureIgnoreCase))
                {
                    list.Remove(xElement);
                }
            }
            RulesDefinition.Deepen(orCreateChildElement, list);
        }
        /// <summary>
        /// Replaces the element references.
        /// </summary>
        /// <param name="elements">The elements collection.</param>
        /// <param name="oldElementId">The old element id.</param>
        /// <param name="newElementId">The new element id.</param>
        private void ReplaceElementReferences(System.Collections.Generic.IEnumerable<XElement> elements, ID oldElementId, ID newElementId)
        {
            Assert.ArgumentNotNull(elements, "elements");
            foreach (XElement current in elements)
            {
                if (current.GetAttributeValue("id") == oldElementId.ToString())
                {
                    current.Attributes().Remove();
                    current.Add(new XAttribute("id", newElementId));
                    current.Add(new XAttribute("uid", ID.NewID.ToShortID().ToString()));
                }
            }
        }
    }
}