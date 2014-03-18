using Hedgehog.SC71Upgrade.Rules;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Hedgehog.SC71Upgrade.Data.Fields
{
    public class RulesField : Sitecore.Data.Fields.CustomField
    {
        private XDocument rulesDefinitionDocument;
        private XDocument RulesDefinitionDocument
        {
            get
            {
                if (this.rulesDefinitionDocument == null && !string.IsNullOrEmpty(base.Value))
                {
                    this.rulesDefinitionDocument = XDocument.Parse(base.Value);
                }
                return this.rulesDefinitionDocument;
            }
        }
        public RulesField(Sitecore.Data.Fields.Field innerField)
            : base(innerField)
        {
            Assert.ArgumentNotNull(innerField, "innerField");
        }
        public RulesField(Sitecore.Data.Fields.Field innerField, string runtimeValue)
            : base(innerField, runtimeValue)
        {
            Assert.ArgumentNotNull(innerField, "innerField");
            Assert.ArgumentNotNull(runtimeValue, "runtimeValue");
        }
        public static implicit operator RulesField(Sitecore.Data.Fields.Field field)
        {
            if (field != null)
            {
                return new RulesField(field);
            }
            return null;
        }
        public override void Relink(ItemLink itemLink, Item newLink)
        {
            Assert.ArgumentNotNull(itemLink, "itemLink");
            Assert.ArgumentNotNull(newLink, "newLink");
            RulesDefinition rulesDefinition = new RulesDefinition(this.RulesDefinitionDocument.ToString());
            if (rulesDefinition.IsActionReferenced(itemLink.TargetItemID))
            {
                rulesDefinition.ReplaceActionReferences(itemLink.TargetItemID, newLink.ID);
            }
            else
            {
                if (rulesDefinition.IsConditionReferenced(itemLink.TargetItemID))
                {
                    rulesDefinition.ReplaceConditionReferences(itemLink.TargetItemID, newLink.ID);
                }
                else
                {
                    if (rulesDefinition.IsItemReferenced(itemLink.TargetItemID))
                    {
                        rulesDefinition.ReplaceItemReferences(itemLink.TargetItemID, newLink.ID);
                    }
                }
            }
            this.rulesDefinitionDocument = rulesDefinition.Document;
            base.Value = ((this.RulesDefinitionDocument != null) ? this.RulesDefinitionDocument.ToString() : string.Empty);
        }
        public override void RemoveLink(ItemLink itemLink)
        {
            Assert.ArgumentNotNull(itemLink, "itemLink");
            RulesDefinition rulesDefinition = new RulesDefinition(this.RulesDefinitionDocument.ToString());
            if (rulesDefinition.IsActionReferenced(itemLink.TargetItemID))
            {
                rulesDefinition.RemoveActionReferences(itemLink.TargetItemID);
            }
            else
            {
                if (rulesDefinition.IsConditionReferenced(itemLink.TargetItemID))
                {
                    rulesDefinition.RemoveConditionReferences(itemLink.TargetItemID);
                }
                else
                {
                    if (rulesDefinition.IsItemReferenced(itemLink.TargetItemID))
                    {
                        rulesDefinition.RemoveItemReferences(itemLink.TargetItemID);
                    }
                }
            }
            this.rulesDefinitionDocument = rulesDefinition.Document;
            base.Value = ((this.RulesDefinitionDocument != null) ? this.RulesDefinitionDocument.ToString() : string.Empty);
        }
        public override void ValidateLinks(LinksValidationResult result)
        {
            Sitecore.Data.Database database = base.InnerField.Database;
            if (database == null)
            {
                return;
            }
            if (this.RulesDefinitionDocument != null)
            {
                RulesDefinition rulesDefinition = new RulesDefinition(this.RulesDefinitionDocument.ToString());
                System.Collections.Generic.List<ID> list = new System.Collections.Generic.List<ID>();
                list.AddRange(rulesDefinition.GetReferencedActions());
                list.AddRange(rulesDefinition.GetReferencedConditions());
                list.AddRange(rulesDefinition.GetReferencedItems());
                foreach (ID current in list)
                {
                    Item item = database.GetItem(current);
                    if (item != null)
                    {
                        result.AddValidLink(item, item.Paths.Path);
                    }
                    else
                    {
                        result.AddBrokenLink(base.Value);
                    }
                }
            }
        }
    }
}