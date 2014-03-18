# Sitecore 7.1 Upgrade Fix #

This is a fix for the Sitecore 7.1 upgrade process, where Rules fields may not have their Source (Rules Context) updated correctly during the upgrade.

## How to Fix ##
Follow the initial steps (Steps 1 to 9) in the Sitecore update installation instructions [here](http://sdn.sitecore.net/Products/Sitecore%20V5/Sitecore%20CMS%207/Update/7_1_rev_130926/Upgrade_Instructions.aspx).<br/>
Before installing the Sitecore 7.1 update package in the Update Installation Wizard (Step 10,11):- 
 
- copy the `Hedgehog.SC71Upgrade.DLL` file from the `__install files` directory into your Website's `/bin` directory.

- make a change to the `App_Config/FieldTypes.config` file with the following change.
 - change `<fieldType name="Rules" type="Sitecore.Data.Fields.TextField,Sitecore.Kernel" resizable="true" />` to
  `<fieldType name="Rules" type="Hedgehog.SC71Upgrade.Data.Fields.RulesField,Hedgehog.SC71Upgrade" resizable="true" />`
 - Note: This change can be seen in the example file located here `__install files/FieldTypes.config`.

- Now install the normal update package (Step 10,11 in the [Sitecore update installation instructions](http://sdn.sitecore.net/Products/Sitecore%20V5/Sitecore%20CMS%207/Update/7_1_rev_130926/Upgrade_Instructions.aspx)) and complete the remaining steps for the upgrade.

## What is the problem with the normal upgrade? ##
I discovered that the [Sitecore Update Installation Post Steps](http://www.seanholmesby.com/sitecore-upgrade-post-step-scripts/) for Sitecore 7.1 was trying to find Actions and Conditions used in Rules fields by using the Link Database.

The problem here is that while these steps are run, the site is still using Sitecore 7.0's `FieldTypes.config` file, which determines how the Links Database uses field values for references to other items. Here, the Rules Field uses the `TextField` type, which...long story short...means that Actions and Conditions aren't 'Linked' to the Rules Field that's using it (according to the Links Database).

Sitecore 7.1 fixes this.... but the installation Post step (using Sitecore 7.0's configs and code) can't find any Actions and Conditions used by the Rules....as it's doing a `linksDatabase.GetReferrers(ruleItem)`.

## What does the fix do? ##
The fix is basically telling Sitecore's Links Database how to properly deal with Rules fields. It's a duplicate of Sitecore 7.1's RulesField code, self contained in the `Hedgehog.SC71Upgrade.DLL`.<br />
The Installation Post Steps can then correctly find the links between Rules fields and Actions and Conditions.... so it can update the Rules Context items appropriately.

## Post Install ##
One of the further steps in the Update Installation Instructions is to update the `FieldTypes.config` with Sitecore's changes. Because our code has already been run by this stage, you can overwrite our changes with Sitecore's suggested one.<br />
You can also remove the `Hedgehog.SC71Upgrade.DLL` file as well, as it will no longer be used.

## What if I've already upgraded? ##
You can manually update the Rules Context's for your custom Actions and Conditions by following this blog post - [How to fix missing Conditions and Actions in Rules fields in Sitecore 7.1](http://www.seanholmesby.com/how-to-fix-missing-conditions-and-actions-in-rules-fields-in-sitecore-7-1/)