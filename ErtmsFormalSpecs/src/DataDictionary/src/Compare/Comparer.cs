// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System.Collections.Generic;
using DataDictionary.Generated;
using acceptor = HistoricalData.Generated.acceptor;

namespace DataDictionary.Compare
{
    public static class Comparer
    {
        /// <summary>
        /// Compares two Namable and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareNamable(Generated.Namable obj, Generated.Namable other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            if (!CompareUtil.canonicalStringEquality(obj.getName(), other.getName()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Name", other.getName(), obj.getName()));
            }
        }

        /// <summary>
        /// Compares two ReferencesParagraph and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareReferencesParagraph(Generated.ReferencesParagraph obj, Generated.ReferencesParagraph other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (obj.allRequirements() != null)
            {
                if (other.allRequirements() != null)
                {
                    foreach (Generated.ReqRef subElement in obj.allRequirements())
                    {
                        bool compared = false;
                        foreach (Generated.ReqRef otherElement in other.allRequirements())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareReqRef(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Requirements", "", subElement.Name));
                        }
                    }

                    foreach (Generated.ReqRef otherElement in other.allRequirements())
                    {
                        bool found = false;
                        foreach (Generated.ReqRef subElement in obj.allRequirements())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Requirements", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Generated.ReqRef subElement in obj.allRequirements())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Requirements", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allRequirements() != null)
                {
                    foreach (Generated.ReqRef otherElement in other.allRequirements())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Requirements", otherElement.Name));
                    }
                }
            }
            if (!CompareUtil.canonicalStringEquality(obj.getComment(), other.getComment()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Comment", other.getComment(), obj.getComment()));
            }
        }

        /// <summary>
        /// Compares two ReqRelated and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareReqRelated(Generated.ReqRelated obj, Generated.ReqRelated other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareReferencesParagraph(obj, other, diff);

            if (obj.getImplemented() != other.getImplemented())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Implemented", other.getImplemented().ToString(), obj.getImplemented().ToString()));
            }
            if (obj.getVerified() != other.getVerified())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Verified", other.getVerified().ToString(), obj.getVerified().ToString()));
            }
            if (obj.getNeedsRequirement() != other.getNeedsRequirement())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "NeedsRequirement", other.getNeedsRequirement().ToString(), obj.getNeedsRequirement().ToString()));
            }
        }

        /// <summary>
        /// Compares two Dictionary and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareDictionary(Generated.Dictionary obj, Generated.Dictionary other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            if (obj.allSpecifications() != null)
            {
                if (other.allSpecifications() != null)
                {
                    foreach (Generated.Specification subElement in obj.allSpecifications())
                    {
                        bool compared = false;
                        foreach (Generated.Specification otherElement in other.allSpecifications())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareSpecification(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Specifications", "", subElement.Name));
                        }
                    }

                    foreach (Generated.Specification otherElement in other.allSpecifications())
                    {
                        bool found = false;
                        foreach (Generated.Specification subElement in obj.allSpecifications())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Specifications", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Generated.Specification subElement in obj.allSpecifications())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Specifications", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allSpecifications() != null)
                {
                    foreach (Generated.Specification otherElement in other.allSpecifications())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Specifications", otherElement.Name));
                    }
                }
            }
            if (obj.allRequirementSets() != null)
            {
                if (other.allRequirementSets() != null)
                {
                    foreach (RequirementSet subElement in obj.allRequirementSets())
                    {
                        bool compared = false;
                        foreach (RequirementSet otherElement in other.allRequirementSets())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareRequirementSet(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "RequirementSets", "", subElement.Name));
                        }
                    }

                    foreach (RequirementSet otherElement in other.allRequirementSets())
                    {
                        bool found = false;
                        foreach (RequirementSet subElement in obj.allRequirementSets())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "RequirementSets", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (RequirementSet subElement in obj.allRequirementSets())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "RequirementSets", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allRequirementSets() != null)
                {
                    foreach (RequirementSet otherElement in other.allRequirementSets())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "RequirementSets", otherElement.Name));
                    }
                }
            }
            if (obj.allRuleDisablings() != null)
            {
                if (other.allRuleDisablings() != null)
                {
                    foreach (RuleDisabling subElement in obj.allRuleDisablings())
                    {
                        bool compared = false;
                        foreach (RuleDisabling otherElement in other.allRuleDisablings())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareRuleDisabling(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "RuleDisablings", "", subElement.Name));
                        }
                    }

                    foreach (RuleDisabling otherElement in other.allRuleDisablings())
                    {
                        bool found = false;
                        foreach (RuleDisabling subElement in obj.allRuleDisablings())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "RuleDisablings", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (RuleDisabling subElement in obj.allRuleDisablings())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "RuleDisablings", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allRuleDisablings() != null)
                {
                    foreach (RuleDisabling otherElement in other.allRuleDisablings())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "RuleDisablings", otherElement.Name));
                    }
                }
            }
            if (obj.allNameSpaces() != null)
            {
                if (other.allNameSpaces() != null)
                {
                    foreach (NameSpace subElement in obj.allNameSpaces())
                    {
                        bool compared = false;
                        foreach (NameSpace otherElement in other.allNameSpaces())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareNameSpace(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "NameSpaces", "", subElement.Name));
                        }
                    }

                    foreach (NameSpace otherElement in other.allNameSpaces())
                    {
                        bool found = false;
                        foreach (NameSpace subElement in obj.allNameSpaces())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "NameSpaces", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (NameSpace subElement in obj.allNameSpaces())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "NameSpaces", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allNameSpaces() != null)
                {
                    foreach (NameSpace otherElement in other.allNameSpaces())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "NameSpaces", otherElement.Name));
                    }
                }
            }
            if (obj.allNameSpaceRefs() != null)
            {
                if (other.allNameSpaceRefs() != null)
                {
                    foreach (NameSpaceRef subElement in obj.allNameSpaceRefs())
                    {
                        bool compared = false;
                        foreach (NameSpaceRef otherElement in other.allNameSpaceRefs())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareNameSpaceRef(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "NameSpaceRefs", "", subElement.Name));
                        }
                    }

                    foreach (NameSpaceRef otherElement in other.allNameSpaceRefs())
                    {
                        bool found = false;
                        foreach (NameSpaceRef subElement in obj.allNameSpaceRefs())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "NameSpaceRefs", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (NameSpaceRef subElement in obj.allNameSpaceRefs())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "NameSpaceRefs", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allNameSpaceRefs() != null)
                {
                    foreach (NameSpaceRef otherElement in other.allNameSpaceRefs())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "NameSpaceRefs", otherElement.Name));
                    }
                }
            }
            if (obj.allTests() != null)
            {
                if (other.allTests() != null)
                {
                    foreach (Frame subElement in obj.allTests())
                    {
                        bool compared = false;
                        foreach (Frame otherElement in other.allTests())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareFrame(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Tests", "", subElement.Name));
                        }
                    }

                    foreach (Frame otherElement in other.allTests())
                    {
                        bool found = false;
                        foreach (Frame subElement in obj.allTests())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Tests", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Frame subElement in obj.allTests())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Tests", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allTests() != null)
                {
                    foreach (Frame otherElement in other.allTests())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Tests", otherElement.Name));
                    }
                }
            }
            if (obj.allTestRefs() != null)
            {
                if (other.allTestRefs() != null)
                {
                    foreach (FrameRef subElement in obj.allTestRefs())
                    {
                        bool compared = false;
                        foreach (FrameRef otherElement in other.allTestRefs())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareFrameRef(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "TestRefs", "", subElement.Name));
                        }
                    }

                    foreach (FrameRef otherElement in other.allTestRefs())
                    {
                        bool found = false;
                        foreach (FrameRef subElement in obj.allTestRefs())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "TestRefs", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (FrameRef subElement in obj.allTestRefs())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "TestRefs", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allTestRefs() != null)
                {
                    foreach (FrameRef otherElement in other.allTestRefs())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "TestRefs", otherElement.Name));
                    }
                }
            }
            if (obj.getTranslationDictionary() == null)
            {
                if (other.getTranslationDictionary() != null)
                {
                    diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "TranslationDictionary", ""));
                }
            }
            else
            {
                compareTranslationDictionary(obj.getTranslationDictionary(), other.getTranslationDictionary(), diff);
            }
            if (obj.getShortcutDictionary() == null)
            {
                if (other.getShortcutDictionary() != null)
                {
                    diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "ShortcutDictionary", ""));
                }
            }
            else
            {
                compareShortcutDictionary(obj.getShortcutDictionary(), other.getShortcutDictionary(), diff);
            }
            if (!CompareUtil.canonicalStringEquality(obj.getXsi(), other.getXsi()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Xsi", other.getXsi(), obj.getXsi()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getXsiLocation(), other.getXsiLocation()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "XsiLocation", other.getXsiLocation(), obj.getXsiLocation()));
            }
        }

        /// <summary>
        /// Compares two RuleDisabling and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareRuleDisabling(RuleDisabling obj, RuleDisabling other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareReqRelated(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getRule(), other.getRule()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Rule", other.getRule(), obj.getRule()));
            }
        }

        /// <summary>
        /// Compares two NameSpaceRef and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareNameSpaceRef(NameSpaceRef obj, NameSpaceRef other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);
        }

        /// <summary>
        /// Compares two NameSpace and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareNameSpace(NameSpace obj, NameSpace other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (obj.allNameSpaces() != null)
            {
                if (other.allNameSpaces() != null)
                {
                    foreach (NameSpace subElement in obj.allNameSpaces())
                    {
                        bool compared = false;
                        foreach (NameSpace otherElement in other.allNameSpaces())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareNameSpace(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "NameSpaces", "", subElement.Name));
                        }
                    }

                    foreach (NameSpace otherElement in other.allNameSpaces())
                    {
                        bool found = false;
                        foreach (NameSpace subElement in obj.allNameSpaces())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "NameSpaces", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (NameSpace subElement in obj.allNameSpaces())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "NameSpaces", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allNameSpaces() != null)
                {
                    foreach (NameSpace otherElement in other.allNameSpaces())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "NameSpaces", otherElement.Name));
                    }
                }
            }
            if (obj.allNameSpaceRefs() != null)
            {
                if (other.allNameSpaceRefs() != null)
                {
                    foreach (NameSpaceRef subElement in obj.allNameSpaceRefs())
                    {
                        bool compared = false;
                        foreach (NameSpaceRef otherElement in other.allNameSpaceRefs())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareNameSpaceRef(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "NameSpaceRefs", "", subElement.Name));
                        }
                    }

                    foreach (NameSpaceRef otherElement in other.allNameSpaceRefs())
                    {
                        bool found = false;
                        foreach (NameSpaceRef subElement in obj.allNameSpaceRefs())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "NameSpaceRefs", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (NameSpaceRef subElement in obj.allNameSpaceRefs())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "NameSpaceRefs", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allNameSpaceRefs() != null)
                {
                    foreach (NameSpaceRef otherElement in other.allNameSpaceRefs())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "NameSpaceRefs", otherElement.Name));
                    }
                }
            }
            if (obj.allRanges() != null)
            {
                if (other.allRanges() != null)
                {
                    foreach (Range subElement in obj.allRanges())
                    {
                        bool compared = false;
                        foreach (Range otherElement in other.allRanges())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareRange(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Ranges", "", subElement.Name));
                        }
                    }

                    foreach (Range otherElement in other.allRanges())
                    {
                        bool found = false;
                        foreach (Range subElement in obj.allRanges())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Ranges", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Range subElement in obj.allRanges())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Ranges", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allRanges() != null)
                {
                    foreach (Range otherElement in other.allRanges())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Ranges", otherElement.Name));
                    }
                }
            }
            if (obj.allEnumerations() != null)
            {
                if (other.allEnumerations() != null)
                {
                    foreach (Enum subElement in obj.allEnumerations())
                    {
                        bool compared = false;
                        foreach (Enum otherElement in other.allEnumerations())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareEnum(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Enumerations", "", subElement.Name));
                        }
                    }

                    foreach (Enum otherElement in other.allEnumerations())
                    {
                        bool found = false;
                        foreach (Enum subElement in obj.allEnumerations())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Enumerations", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Enum subElement in obj.allEnumerations())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Enumerations", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allEnumerations() != null)
                {
                    foreach (Enum otherElement in other.allEnumerations())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Enumerations", otherElement.Name));
                    }
                }
            }
            if (obj.allStructures() != null)
            {
                if (other.allStructures() != null)
                {
                    foreach (Structure subElement in obj.allStructures())
                    {
                        bool compared = false;
                        foreach (Structure otherElement in other.allStructures())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareStructure(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Structures", "", subElement.Name));
                        }
                    }

                    foreach (Structure otherElement in other.allStructures())
                    {
                        bool found = false;
                        foreach (Structure subElement in obj.allStructures())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Structures", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Structure subElement in obj.allStructures())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Structures", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allStructures() != null)
                {
                    foreach (Structure otherElement in other.allStructures())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Structures", otherElement.Name));
                    }
                }
            }
            if (obj.allCollections() != null)
            {
                if (other.allCollections() != null)
                {
                    foreach (Collection subElement in obj.allCollections())
                    {
                        bool compared = false;
                        foreach (Collection otherElement in other.allCollections())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareCollection(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Collections", "", subElement.Name));
                        }
                    }

                    foreach (Collection otherElement in other.allCollections())
                    {
                        bool found = false;
                        foreach (Collection subElement in obj.allCollections())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Collections", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Collection subElement in obj.allCollections())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Collections", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allCollections() != null)
                {
                    foreach (Collection otherElement in other.allCollections())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Collections", otherElement.Name));
                    }
                }
            }
            if (obj.allStateMachines() != null)
            {
                if (other.allStateMachines() != null)
                {
                    foreach (StateMachine subElement in obj.allStateMachines())
                    {
                        bool compared = false;
                        foreach (StateMachine otherElement in other.allStateMachines())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareStateMachine(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "StateMachines", "", subElement.Name));
                        }
                    }

                    foreach (StateMachine otherElement in other.allStateMachines())
                    {
                        bool found = false;
                        foreach (StateMachine subElement in obj.allStateMachines())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "StateMachines", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (StateMachine subElement in obj.allStateMachines())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "StateMachines", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allStateMachines() != null)
                {
                    foreach (StateMachine otherElement in other.allStateMachines())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "StateMachines", otherElement.Name));
                    }
                }
            }
            if (obj.allFunctions() != null)
            {
                if (other.allFunctions() != null)
                {
                    foreach (Function subElement in obj.allFunctions())
                    {
                        bool compared = false;
                        foreach (Function otherElement in other.allFunctions())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareFunction(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Functions", "", subElement.Name));
                        }
                    }

                    foreach (Function otherElement in other.allFunctions())
                    {
                        bool found = false;
                        foreach (Function subElement in obj.allFunctions())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Functions", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Function subElement in obj.allFunctions())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Functions", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allFunctions() != null)
                {
                    foreach (Function otherElement in other.allFunctions())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Functions", otherElement.Name));
                    }
                }
            }
            if (obj.allProcedures() != null)
            {
                if (other.allProcedures() != null)
                {
                    foreach (Procedure subElement in obj.allProcedures())
                    {
                        bool compared = false;
                        foreach (Procedure otherElement in other.allProcedures())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareProcedure(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Procedures", "", subElement.Name));
                        }
                    }

                    foreach (Procedure otherElement in other.allProcedures())
                    {
                        bool found = false;
                        foreach (Procedure subElement in obj.allProcedures())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Procedures", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Procedure subElement in obj.allProcedures())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Procedures", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allProcedures() != null)
                {
                    foreach (Procedure otherElement in other.allProcedures())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Procedures", otherElement.Name));
                    }
                }
            }
            if (obj.allVariables() != null)
            {
                if (other.allVariables() != null)
                {
                    foreach (Variable subElement in obj.allVariables())
                    {
                        bool compared = false;
                        foreach (Variable otherElement in other.allVariables())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareVariable(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Variables", "", subElement.Name));
                        }
                    }

                    foreach (Variable otherElement in other.allVariables())
                    {
                        bool found = false;
                        foreach (Variable subElement in obj.allVariables())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Variables", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Variable subElement in obj.allVariables())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Variables", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allVariables() != null)
                {
                    foreach (Variable otherElement in other.allVariables())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Variables", otherElement.Name));
                    }
                }
            }
            if (obj.allRules() != null)
            {
                if (other.allRules() != null)
                {
                    foreach (Rule subElement in obj.allRules())
                    {
                        bool compared = false;
                        foreach (Rule otherElement in other.allRules())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareRule(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Rules", "", subElement.Name));
                        }
                    }

                    foreach (Rule otherElement in other.allRules())
                    {
                        bool found = false;
                        foreach (Rule subElement in obj.allRules())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Rules", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Rule subElement in obj.allRules())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Rules", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allRules() != null)
                {
                    foreach (Rule otherElement in other.allRules())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Rules", otherElement.Name));
                    }
                }
            }
            if (!CompareUtil.canonicalStringEquality(obj.getComment(), other.getComment()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Comment", other.getComment(), obj.getComment()));
            }
        }

        /// <summary>
        /// Compares two ReqRef and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareReqRef(Generated.ReqRef obj, Generated.ReqRef other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            if (!CompareUtil.canonicalStringEquality(obj.getComment(), other.getComment()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Comment", other.getComment(), obj.getComment()));
            }
        }

        /// <summary>
        /// Compares two Type and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareType(Type obj, Type other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareReqRelated(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getDefault(), other.getDefault()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Default", other.getDefault(), obj.getDefault()));
            }
            if (obj.getWidth() != other.getWidth())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Width", other.getWidth().ToString(), obj.getWidth().ToString()));
            }
            if (obj.getHeight() != other.getHeight())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Height", other.getHeight().ToString(), obj.getHeight().ToString()));
            }
            if (obj.getX() != other.getX())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "X", other.getX().ToString(), obj.getX().ToString()));
            }
            if (obj.getY() != other.getY())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Y", other.getY().ToString(), obj.getY().ToString()));
            }
            if (obj.getHidden() != other.getHidden())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Hidden", other.getHidden().ToString(), obj.getHidden().ToString()));
            }
            if (obj.getPinned() != other.getPinned())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Pinned", other.getPinned().ToString(), obj.getPinned().ToString()));
            }
        }

        /// <summary>
        /// Compares two Enum and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareEnum(Enum obj, Enum other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareType(obj, other, diff);

            if (obj.allValues() != null)
            {
                if (other.allValues() != null)
                {
                    int i = 0;
                    while (i < obj.countValues() && i < other.countValues())
                    {
                        EnumValue element = obj.getValues(i);
                        EnumValue otherElement = other.getValues(i);
                        compareEnumValue(element, otherElement, diff);
                        i += 1;
                    }
                    while (i < obj.countValues())
                    {
                        diff.appendChanges(new Diff(obj.getValues(i), acceptor.ChangeOperationEnum.aAdd, "Values", "", obj.getValues(i).Name));
                        i += 1;
                    }
                    while (i < other.countValues())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Values", other.getValues(i).Name));
                        i += 1;
                    }
                }
                else
                {
                    foreach (EnumValue subElement in obj.allValues())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Values", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allValues() != null)
                {
                    foreach (EnumValue otherElement in other.allValues())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Values", otherElement.Name));
                    }
                }
            }
            if (obj.allSubEnums() != null)
            {
                if (other.allSubEnums() != null)
                {
                    foreach (Enum subElement in obj.allSubEnums())
                    {
                        bool compared = false;
                        foreach (Enum otherElement in other.allSubEnums())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareEnum(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SubEnums", "", subElement.Name));
                        }
                    }

                    foreach (Enum otherElement in other.allSubEnums())
                    {
                        bool found = false;
                        foreach (Enum subElement in obj.allSubEnums())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SubEnums", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Enum subElement in obj.allSubEnums())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SubEnums", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allSubEnums() != null)
                {
                    foreach (Enum otherElement in other.allSubEnums())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SubEnums", otherElement.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two EnumValue and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareEnumValue(EnumValue obj, EnumValue other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getValue(), other.getValue()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Value", other.getValue(), obj.getValue()));
            }
            if (obj.getForbidArithmeticOperation() != other.getForbidArithmeticOperation())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "ForbidArithmeticOperation", other.getForbidArithmeticOperation().ToString(), obj.getForbidArithmeticOperation().ToString()));
            }
        }

        /// <summary>
        /// Compares two Range and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareRange(Range obj, Range other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareType(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getMinValue(), other.getMinValue()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "MinValue", other.getMinValue(), obj.getMinValue()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getMaxValue(), other.getMaxValue()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "MaxValue", other.getMaxValue(), obj.getMaxValue()));
            }
            if (obj.allSpecialValues() != null)
            {
                if (other.allSpecialValues() != null)
                {
                    int i = 0;
                    while (i < obj.countSpecialValues() && i < other.countSpecialValues())
                    {
                        EnumValue element = obj.getSpecialValues(i);
                        EnumValue otherElement = other.getSpecialValues(i);
                        compareEnumValue(element, otherElement, diff);
                        i += 1;
                    }
                    while (i < obj.countSpecialValues())
                    {
                        diff.appendChanges(new Diff(obj.getSpecialValues(i), acceptor.ChangeOperationEnum.aAdd, "SpecialValues", "", obj.getSpecialValues(i).Name));
                        i += 1;
                    }
                    while (i < other.countSpecialValues())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SpecialValues", other.getSpecialValues(i).Name));
                        i += 1;
                    }
                }
                else
                {
                    foreach (EnumValue subElement in obj.allSpecialValues())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SpecialValues", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allSpecialValues() != null)
                {
                    foreach (EnumValue otherElement in other.allSpecialValues())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SpecialValues", otherElement.Name));
                    }
                }
            }
            if (obj.getPrecision() != other.getPrecision())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Precision", other.getPrecision().ToString(), obj.getPrecision().ToString()));
            }
        }

        /// <summary>
        /// Compares two Structure and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareStructure(Structure obj, Structure other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareType(obj, other, diff);

            if (obj.allElements() != null)
            {
                if (other.allElements() != null)
                {
                    foreach (StructureElement subElement in obj.allElements())
                    {
                        bool compared = false;
                        foreach (StructureElement otherElement in other.allElements())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareStructureElement(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Elements", "", subElement.Name));
                        }
                    }

                    foreach (StructureElement otherElement in other.allElements())
                    {
                        bool found = false;
                        foreach (StructureElement subElement in obj.allElements())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Elements", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (StructureElement subElement in obj.allElements())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Elements", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allElements() != null)
                {
                    foreach (StructureElement otherElement in other.allElements())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Elements", otherElement.Name));
                    }
                }
            }
            if (obj.allProcedures() != null)
            {
                if (other.allProcedures() != null)
                {
                    foreach (Procedure subElement in obj.allProcedures())
                    {
                        bool compared = false;
                        foreach (Procedure otherElement in other.allProcedures())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareProcedure(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Procedures", "", subElement.Name));
                        }
                    }

                    foreach (Procedure otherElement in other.allProcedures())
                    {
                        bool found = false;
                        foreach (Procedure subElement in obj.allProcedures())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Procedures", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Procedure subElement in obj.allProcedures())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Procedures", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allProcedures() != null)
                {
                    foreach (Procedure otherElement in other.allProcedures())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Procedures", otherElement.Name));
                    }
                }
            }
            if (obj.allStateMachines() != null)
            {
                if (other.allStateMachines() != null)
                {
                    foreach (StateMachine subElement in obj.allStateMachines())
                    {
                        bool compared = false;
                        foreach (StateMachine otherElement in other.allStateMachines())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareStateMachine(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "StateMachines", "", subElement.Name));
                        }
                    }

                    foreach (StateMachine otherElement in other.allStateMachines())
                    {
                        bool found = false;
                        foreach (StateMachine subElement in obj.allStateMachines())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "StateMachines", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (StateMachine subElement in obj.allStateMachines())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "StateMachines", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allStateMachines() != null)
                {
                    foreach (StateMachine otherElement in other.allStateMachines())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "StateMachines", otherElement.Name));
                    }
                }
            }
            if (obj.allRules() != null)
            {
                if (other.allRules() != null)
                {
                    foreach (Rule subElement in obj.allRules())
                    {
                        bool compared = false;
                        foreach (Rule otherElement in other.allRules())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareRule(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Rules", "", subElement.Name));
                        }
                    }

                    foreach (Rule otherElement in other.allRules())
                    {
                        bool found = false;
                        foreach (Rule subElement in obj.allRules())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Rules", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Rule subElement in obj.allRules())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Rules", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allRules() != null)
                {
                    foreach (Rule otherElement in other.allRules())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Rules", otherElement.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two StructureElement and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareStructureElement(StructureElement obj, StructureElement other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareReqRelated(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getTypeName(), other.getTypeName()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "TypeName", other.getTypeName(), obj.getTypeName()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getDefault(), other.getDefault()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Default", other.getDefault(), obj.getDefault()));
            }
            if (obj.getMode() != other.getMode())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Mode", other.getMode().ToString(), obj.getMode().ToString()));
            }
        }

        /// <summary>
        /// Compares two Collection and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareCollection(Collection obj, Collection other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareType(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getTypeName(), other.getTypeName()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "TypeName", other.getTypeName(), obj.getTypeName()));
            }
            if (obj.getMaxSize() != other.getMaxSize())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "MaxSize", other.getMaxSize().ToString(), obj.getMaxSize().ToString()));
            }
        }

        /// <summary>
        /// Compares two Function and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareFunction(Function obj, Function other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareType(obj, other, diff);

            if (obj.allParameters() != null)
            {
                if (other.allParameters() != null)
                {
                    foreach (Generated.Parameter subElement in obj.allParameters())
                    {
                        bool compared = false;
                        foreach (Generated.Parameter otherElement in other.allParameters())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareParameter(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Parameters", "", subElement.Name));
                        }
                    }

                    foreach (Generated.Parameter otherElement in other.allParameters())
                    {
                        bool found = false;
                        foreach (Generated.Parameter subElement in obj.allParameters())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Parameters", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Generated.Parameter subElement in obj.allParameters())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Parameters", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allParameters() != null)
                {
                    foreach (Generated.Parameter otherElement in other.allParameters())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Parameters", otherElement.Name));
                    }
                }
            }
            if (obj.allCases() != null)
            {
                if (other.allCases() != null)
                {
                    int i = 0;
                    while (i < obj.countCases() && i < other.countCases())
                    {
                        Case element = obj.getCases(i);
                        Case otherElement = other.getCases(i);
                        compareCase(element, otherElement, diff);
                        i += 1;
                    }
                    while (i < obj.countCases())
                    {
                        diff.appendChanges(new Diff(obj.getCases(i), acceptor.ChangeOperationEnum.aAdd, "Cases", "", obj.getCases(i).Name));
                        i += 1;
                    }
                    while (i < other.countCases())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Cases", other.getCases(i).Name));
                        i += 1;
                    }
                }
                else
                {
                    foreach (Case subElement in obj.allCases())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Cases", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allCases() != null)
                {
                    foreach (Case otherElement in other.allCases())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Cases", otherElement.Name));
                    }
                }
            }
            if (!CompareUtil.canonicalStringEquality(obj.getTypeName(), other.getTypeName()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "TypeName", other.getTypeName(), obj.getTypeName()));
            }
            if (obj.getCacheable() != other.getCacheable())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Cacheable", other.getCacheable().ToString(), obj.getCacheable().ToString()));
            }
        }

        /// <summary>
        /// Compares two Parameter and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareParameter(Generated.Parameter obj, Generated.Parameter other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getTypeName(), other.getTypeName()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "TypeName", other.getTypeName(), obj.getTypeName()));
            }
        }

        /// <summary>
        /// Compares two Case and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareCase(Case obj, Case other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (obj.allPreConditions() != null)
            {
                if (other.allPreConditions() != null)
                {
                    int i = 0;
                    while (i < obj.countPreConditions() && i < other.countPreConditions())
                    {
                        PreCondition element = obj.getPreConditions(i);
                        PreCondition otherElement = other.getPreConditions(i);
                        comparePreCondition(element, otherElement, diff);
                        i += 1;
                    }
                    while (i < obj.countPreConditions())
                    {
                        diff.appendChanges(new Diff(obj.getPreConditions(i), acceptor.ChangeOperationEnum.aAdd, "PreConditions", "", obj.getPreConditions(i).Name));
                        i += 1;
                    }
                    while (i < other.countPreConditions())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "PreConditions", other.getPreConditions(i).Name));
                        i += 1;
                    }
                }
                else
                {
                    foreach (PreCondition subElement in obj.allPreConditions())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "PreConditions", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allPreConditions() != null)
                {
                    foreach (PreCondition otherElement in other.allPreConditions())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "PreConditions", otherElement.Name));
                    }
                }
            }
            if (!CompareUtil.canonicalStringEquality(obj.getExpression(), other.getExpression()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Expression", other.getExpression(), obj.getExpression()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getComment(), other.getComment()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Comment", other.getComment(), obj.getComment()));
            }
        }

        /// <summary>
        /// Compares two Procedure and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareProcedure(Procedure obj, Procedure other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareReqRelated(obj, other, diff);

            if (obj.getStateMachine() == null)
            {
                if (other.getStateMachine() != null)
                {
                    diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "StateMachine", ""));
                }
            }
            else
            {
                compareStateMachine(obj.getStateMachine(), other.getStateMachine(), diff);
            }
            if (obj.allRules() != null)
            {
                if (other.allRules() != null)
                {
                    foreach (Rule subElement in obj.allRules())
                    {
                        bool compared = false;
                        foreach (Rule otherElement in other.allRules())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareRule(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Rules", "", subElement.Name));
                        }
                    }

                    foreach (Rule otherElement in other.allRules())
                    {
                        bool found = false;
                        foreach (Rule subElement in obj.allRules())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Rules", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Rule subElement in obj.allRules())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Rules", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allRules() != null)
                {
                    foreach (Rule otherElement in other.allRules())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Rules", otherElement.Name));
                    }
                }
            }
            if (obj.allParameters() != null)
            {
                if (other.allParameters() != null)
                {
                    foreach (Generated.Parameter subElement in obj.allParameters())
                    {
                        bool compared = false;
                        foreach (Generated.Parameter otherElement in other.allParameters())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareParameter(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Parameters", "", subElement.Name));
                        }
                    }

                    foreach (Generated.Parameter otherElement in other.allParameters())
                    {
                        bool found = false;
                        foreach (Generated.Parameter subElement in obj.allParameters())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Parameters", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Generated.Parameter subElement in obj.allParameters())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Parameters", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allParameters() != null)
                {
                    foreach (Generated.Parameter otherElement in other.allParameters())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Parameters", otherElement.Name));
                    }
                }
            }
            if (obj.getWidth() != other.getWidth())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Width", other.getWidth().ToString(), obj.getWidth().ToString()));
            }
            if (obj.getHeight() != other.getHeight())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Height", other.getHeight().ToString(), obj.getHeight().ToString()));
            }
            if (obj.getX() != other.getX())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "X", other.getX().ToString(), obj.getX().ToString()));
            }
            if (obj.getY() != other.getY())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Y", other.getY().ToString(), obj.getY().ToString()));
            }
            if (obj.getHidden() != other.getHidden())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Hidden", other.getHidden().ToString(), obj.getHidden().ToString()));
            }
            if (obj.getPinned() != other.getPinned())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Pinned", other.getPinned().ToString(), obj.getPinned().ToString()));
            }
        }

        /// <summary>
        /// Compares two StateMachine and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareStateMachine(StateMachine obj, StateMachine other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareType(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getInitialState(), other.getInitialState()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "InitialState", other.getInitialState(), obj.getInitialState()));
            }
            if (obj.allStates() != null)
            {
                if (other.allStates() != null)
                {
                    foreach (State subElement in obj.allStates())
                    {
                        bool compared = false;
                        foreach (State otherElement in other.allStates())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareState(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "States", "", subElement.Name));
                        }
                    }

                    foreach (State otherElement in other.allStates())
                    {
                        bool found = false;
                        foreach (State subElement in obj.allStates())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "States", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (State subElement in obj.allStates())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "States", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allStates() != null)
                {
                    foreach (State otherElement in other.allStates())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "States", otherElement.Name));
                    }
                }
            }
            if (obj.allRules() != null)
            {
                if (other.allRules() != null)
                {
                    foreach (Rule subElement in obj.allRules())
                    {
                        bool compared = false;
                        foreach (Rule otherElement in other.allRules())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareRule(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Rules", "", subElement.Name));
                        }
                    }

                    foreach (Rule otherElement in other.allRules())
                    {
                        bool found = false;
                        foreach (Rule subElement in obj.allRules())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Rules", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Rule subElement in obj.allRules())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Rules", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allRules() != null)
                {
                    foreach (Rule otherElement in other.allRules())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Rules", otherElement.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two State and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareState(State obj, State other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareReqRelated(obj, other, diff);

            if (obj.getStateMachine() == null)
            {
                if (other.getStateMachine() != null)
                {
                    diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "StateMachine", ""));
                }
            }
            else
            {
                compareStateMachine(obj.getStateMachine(), other.getStateMachine(), diff);
            }
            if (obj.getWidth() != other.getWidth())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Width", other.getWidth().ToString(), obj.getWidth().ToString()));
            }
            if (obj.getHeight() != other.getHeight())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Height", other.getHeight().ToString(), obj.getHeight().ToString()));
            }
            if (obj.getX() != other.getX())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "X", other.getX().ToString(), obj.getX().ToString()));
            }
            if (obj.getY() != other.getY())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Y", other.getY().ToString(), obj.getY().ToString()));
            }
            if (obj.getPinned() != other.getPinned())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Pinned", other.getPinned().ToString(), obj.getPinned().ToString()));
            }
            if (obj.getEnterAction() == null)
            {
                if (other.getEnterAction() != null)
                {
                    diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "EnterAction", ""));
                }
            }
            else
            {
                compareRule(obj.getEnterAction(), other.getEnterAction(), diff);
            }
            if (obj.getLeaveAction() == null)
            {
                if (other.getLeaveAction() != null)
                {
                    diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "LeaveAction", ""));
                }
            }
            else
            {
                compareRule(obj.getLeaveAction(), other.getLeaveAction(), diff);
            }
        }

        /// <summary>
        /// Compares two Variable and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareVariable(Variable obj, Variable other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareReqRelated(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getTypeName(), other.getTypeName()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "TypeName", other.getTypeName(), obj.getTypeName()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getDefaultValue(), other.getDefaultValue()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "DefaultValue", other.getDefaultValue(), obj.getDefaultValue()));
            }
            if (obj.getVariableMode() != other.getVariableMode())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "VariableMode", other.getVariableMode().ToString(), obj.getVariableMode().ToString()));
            }
            if (obj.allSubVariables() != null)
            {
                if (other.allSubVariables() != null)
                {
                    foreach (Variable subElement in obj.allSubVariables())
                    {
                        bool compared = false;
                        foreach (Variable otherElement in other.allSubVariables())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareVariable(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SubVariables", "", subElement.Name));
                        }
                    }

                    foreach (Variable otherElement in other.allSubVariables())
                    {
                        bool found = false;
                        foreach (Variable subElement in obj.allSubVariables())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SubVariables", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Variable subElement in obj.allSubVariables())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SubVariables", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allSubVariables() != null)
                {
                    foreach (Variable otherElement in other.allSubVariables())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SubVariables", otherElement.Name));
                    }
                }
            }
            if (obj.getWidth() != other.getWidth())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Width", other.getWidth().ToString(), obj.getWidth().ToString()));
            }
            if (obj.getHeight() != other.getHeight())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Height", other.getHeight().ToString(), obj.getHeight().ToString()));
            }
            if (obj.getX() != other.getX())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "X", other.getX().ToString(), obj.getX().ToString()));
            }
            if (obj.getY() != other.getY())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Y", other.getY().ToString(), obj.getY().ToString()));
            }
            if (obj.getHidden() != other.getHidden())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Hidden", other.getHidden().ToString(), obj.getHidden().ToString()));
            }
            if (obj.getPinned() != other.getPinned())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Pinned", other.getPinned().ToString(), obj.getPinned().ToString()));
            }
        }

        /// <summary>
        /// Compares two Rule and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareRule(Rule obj, Rule other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareReqRelated(obj, other, diff);

            if (obj.getPriority() != other.getPriority())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Priority", other.getPriority().ToString(), obj.getPriority().ToString()));
            }
            if (obj.allConditions() != null)
            {
                if (other.allConditions() != null)
                {
                    foreach (RuleCondition subElement in obj.allConditions())
                    {
                        bool compared = false;
                        foreach (RuleCondition otherElement in other.allConditions())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareRuleCondition(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Conditions", "", subElement.Name));
                        }
                    }

                    foreach (RuleCondition otherElement in other.allConditions())
                    {
                        bool found = false;
                        foreach (RuleCondition subElement in obj.allConditions())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Conditions", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (RuleCondition subElement in obj.allConditions())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Conditions", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allConditions() != null)
                {
                    foreach (RuleCondition otherElement in other.allConditions())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Conditions", otherElement.Name));
                    }
                }
            }
            if (obj.getWidth() != other.getWidth())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Width", other.getWidth().ToString(), obj.getWidth().ToString()));
            }
            if (obj.getHeight() != other.getHeight())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Height", other.getHeight().ToString(), obj.getHeight().ToString()));
            }
            if (obj.getX() != other.getX())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "X", other.getX().ToString(), obj.getX().ToString()));
            }
            if (obj.getY() != other.getY())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Y", other.getY().ToString(), obj.getY().ToString()));
            }
            if (obj.getHidden() != other.getHidden())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Hidden", other.getHidden().ToString(), obj.getHidden().ToString()));
            }
            if (obj.getPinned() != other.getPinned())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Pinned", other.getPinned().ToString(), obj.getPinned().ToString()));
            }
        }

        /// <summary>
        /// Compares two RuleCondition and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareRuleCondition(RuleCondition obj, RuleCondition other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareReqRelated(obj, other, diff);

            if (obj.allPreConditions() != null)
            {
                if (other.allPreConditions() != null)
                {
                    int i = 0;
                    while (i < obj.countPreConditions() && i < other.countPreConditions())
                    {
                        PreCondition element = obj.getPreConditions(i);
                        PreCondition otherElement = other.getPreConditions(i);
                        comparePreCondition(element, otherElement, diff);
                        i += 1;
                    }
                    while (i < obj.countPreConditions())
                    {
                        diff.appendChanges(new Diff(obj.getPreConditions(i), acceptor.ChangeOperationEnum.aAdd, "PreConditions", "", obj.getPreConditions(i).Name));
                        i += 1;
                    }
                    while (i < other.countPreConditions())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "PreConditions", other.getPreConditions(i).Name));
                        i += 1;
                    }
                }
                else
                {
                    foreach (PreCondition subElement in obj.allPreConditions())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "PreConditions", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allPreConditions() != null)
                {
                    foreach (PreCondition otherElement in other.allPreConditions())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "PreConditions", otherElement.Name));
                    }
                }
            }
            if (obj.allActions() != null)
            {
                if (other.allActions() != null)
                {
                    int i = 0;
                    while (i < obj.countActions() && i < other.countActions())
                    {
                        Action element = obj.getActions(i);
                        Action otherElement = other.getActions(i);
                        compareAction(element, otherElement, diff);
                        i += 1;
                    }
                    while (i < obj.countActions())
                    {
                        diff.appendChanges(new Diff(obj.getActions(i), acceptor.ChangeOperationEnum.aAdd, "Actions", "", obj.getActions(i).Name));
                        i += 1;
                    }
                    while (i < other.countActions())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Actions", other.getActions(i).Name));
                        i += 1;
                    }
                }
                else
                {
                    foreach (Action subElement in obj.allActions())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Actions", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allActions() != null)
                {
                    foreach (Action otherElement in other.allActions())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Actions", otherElement.Name));
                    }
                }
            }
            if (obj.allSubRules() != null)
            {
                if (other.allSubRules() != null)
                {
                    foreach (Rule subElement in obj.allSubRules())
                    {
                        bool compared = false;
                        foreach (Rule otherElement in other.allSubRules())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareRule(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SubRules", "", subElement.Name));
                        }
                    }

                    foreach (Rule otherElement in other.allSubRules())
                    {
                        bool found = false;
                        foreach (Rule subElement in obj.allSubRules())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SubRules", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Rule subElement in obj.allSubRules())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SubRules", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allSubRules() != null)
                {
                    foreach (Rule otherElement in other.allSubRules())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SubRules", otherElement.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two PreCondition and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void comparePreCondition(PreCondition obj, PreCondition other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            if (!CompareUtil.canonicalStringEquality(obj.getCondition(), other.getCondition()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Condition", other.getCondition(), obj.getCondition()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getComment(), other.getComment()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Comment", other.getComment(), obj.getComment()));
            }
        }

        /// <summary>
        /// Compares two Action and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareAction(Action obj, Action other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            if (!CompareUtil.canonicalStringEquality(obj.getExpression(), other.getExpression()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Expression", other.getExpression(), obj.getExpression()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getComment(), other.getComment()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Comment", other.getComment(), obj.getComment()));
            }
        }

        /// <summary>
        /// Compares two FrameRef and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareFrameRef(FrameRef obj, FrameRef other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);
        }

        /// <summary>
        /// Compares two Frame and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareFrame(Frame obj, Frame other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getCycleDuration(), other.getCycleDuration()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "CycleDuration", other.getCycleDuration(), obj.getCycleDuration()));
            }
            if (obj.allSubSequences() != null)
            {
                if (other.allSubSequences() != null)
                {
                    foreach (SubSequence subElement in obj.allSubSequences())
                    {
                        bool compared = false;
                        foreach (SubSequence otherElement in other.allSubSequences())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareSubSequence(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SubSequences", "", subElement.Name));
                        }
                    }

                    foreach (SubSequence otherElement in other.allSubSequences())
                    {
                        bool found = false;
                        foreach (SubSequence subElement in obj.allSubSequences())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SubSequences", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (SubSequence subElement in obj.allSubSequences())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SubSequences", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allSubSequences() != null)
                {
                    foreach (SubSequence otherElement in other.allSubSequences())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SubSequences", otherElement.Name));
                    }
                }
            }
            if (!CompareUtil.canonicalStringEquality(obj.getComment(), other.getComment()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Comment", other.getComment(), obj.getComment()));
            }
        }

        /// <summary>
        /// Compares two SubSequence and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareSubSequence(SubSequence obj, SubSequence other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getD_LRBG(), other.getD_LRBG()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "D_LRBG", other.getD_LRBG(), obj.getD_LRBG()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getLevel(), other.getLevel()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Level", other.getLevel(), obj.getLevel()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getMode(), other.getMode()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Mode", other.getMode(), obj.getMode()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getNID_LRBG(), other.getNID_LRBG()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "NID_LRBG", other.getNID_LRBG(), obj.getNID_LRBG()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getQ_DIRLRBG(), other.getQ_DIRLRBG()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Q_DIRLRBG", other.getQ_DIRLRBG(), obj.getQ_DIRLRBG()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getQ_DIRTRAIN(), other.getQ_DIRTRAIN()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Q_DIRTRAIN", other.getQ_DIRTRAIN(), obj.getQ_DIRTRAIN()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getQ_DLRBG(), other.getQ_DLRBG()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Q_DLRBG", other.getQ_DLRBG(), obj.getQ_DLRBG()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getRBC_ID(), other.getRBC_ID()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "RBC_ID", other.getRBC_ID(), obj.getRBC_ID()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getRBCPhone(), other.getRBCPhone()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "RBCPhone", other.getRBCPhone(), obj.getRBCPhone()));
            }
            if (obj.allTestCases() != null)
            {
                if (other.allTestCases() != null)
                {
                    foreach (TestCase subElement in obj.allTestCases())
                    {
                        bool compared = false;
                        foreach (TestCase otherElement in other.allTestCases())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareTestCase(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "TestCases", "", subElement.Name));
                        }
                    }

                    foreach (TestCase otherElement in other.allTestCases())
                    {
                        bool found = false;
                        foreach (TestCase subElement in obj.allTestCases())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "TestCases", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (TestCase subElement in obj.allTestCases())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "TestCases", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allTestCases() != null)
                {
                    foreach (TestCase otherElement in other.allTestCases())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "TestCases", otherElement.Name));
                    }
                }
            }
            if (!CompareUtil.canonicalStringEquality(obj.getComment(), other.getComment()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Comment", other.getComment(), obj.getComment()));
            }
            if (obj.getCompleted() != other.getCompleted())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Completed", other.getCompleted().ToString(), obj.getCompleted().ToString()));
            }
        }

        /// <summary>
        /// Compares two TestCase and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareTestCase(TestCase obj, TestCase other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareReqRelated(obj, other, diff);

            if (obj.getFeature() != other.getFeature())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Feature", other.getFeature().ToString(), obj.getFeature().ToString()));
            }
            if (obj.getCase() != other.getCase())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Case", other.getCase().ToString(), obj.getCase().ToString()));
            }
            if (obj.allSteps() != null)
            {
                if (other.allSteps() != null)
                {
                    foreach (Step subElement in obj.allSteps())
                    {
                        bool compared = false;
                        foreach (Step otherElement in other.allSteps())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareStep(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Steps", "", subElement.Name));
                        }
                    }

                    foreach (Step otherElement in other.allSteps())
                    {
                        bool found = false;
                        foreach (Step subElement in obj.allSteps())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Steps", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Step subElement in obj.allSteps())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Steps", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allSteps() != null)
                {
                    foreach (Step otherElement in other.allSteps())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Steps", otherElement.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two Step and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareStep(Step obj, Step other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareReferencesParagraph(obj, other, diff);

            if (obj.getTCS_Order() != other.getTCS_Order())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "TCS_Order", other.getTCS_Order().ToString(), obj.getTCS_Order().ToString()));
            }
            if (obj.getDistance() != other.getDistance())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Distance", other.getDistance().ToString(), obj.getDistance().ToString()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getDescription(), other.getDescription()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Description", other.getDescription(), obj.getDescription()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getObsoleteComment(), other.getObsoleteComment()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "ObsoleteComment", other.getObsoleteComment(), obj.getObsoleteComment()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getUserComment(), other.getUserComment()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "UserComment", other.getUserComment(), obj.getUserComment()));
            }
            if (obj.getIO() != other.getIO())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "IO", other.getIO().ToString(), obj.getIO().ToString()));
            }
            if (obj.getInterface() != other.getInterface())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Interface", other.getInterface().ToString(), obj.getInterface().ToString()));
            }
            if (obj.getLevelIN() != other.getLevelIN())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "LevelIN", other.getLevelIN().ToString(), obj.getLevelIN().ToString()));
            }
            if (obj.getLevelOUT() != other.getLevelOUT())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "LevelOUT", other.getLevelOUT().ToString(), obj.getLevelOUT().ToString()));
            }
            if (obj.getModeIN() != other.getModeIN())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "ModeIN", other.getModeIN().ToString(), obj.getModeIN().ToString()));
            }
            if (obj.getModeOUT() != other.getModeOUT())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "ModeOUT", other.getModeOUT().ToString(), obj.getModeOUT().ToString()));
            }
            if (obj.getTranslationRequired() != other.getTranslationRequired())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "TranslationRequired", other.getTranslationRequired().ToString(), obj.getTranslationRequired().ToString()));
            }
            if (obj.getTranslated() != other.getTranslated())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Translated", other.getTranslated().ToString(), obj.getTranslated().ToString()));
            }
            if (obj.allSubSteps() != null)
            {
                if (other.allSubSteps() != null)
                {
                    foreach (SubStep subElement in obj.allSubSteps())
                    {
                        bool compared = false;
                        foreach (SubStep otherElement in other.allSubSteps())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareSubStep(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SubSteps", "", subElement.Name));
                        }
                    }

                    foreach (SubStep otherElement in other.allSubSteps())
                    {
                        bool found = false;
                        foreach (SubStep subElement in obj.allSubSteps())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SubSteps", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (SubStep subElement in obj.allSubSteps())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SubSteps", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allSubSteps() != null)
                {
                    foreach (SubStep otherElement in other.allSubSteps())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SubSteps", otherElement.Name));
                    }
                }
            }
            if (obj.allMessages() != null)
            {
                if (other.allMessages() != null)
                {
                    foreach (DBMessage subElement in obj.allMessages())
                    {
                        bool compared = false;
                        foreach (DBMessage otherElement in other.allMessages())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareDBMessage(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Messages", "", subElement.Name));
                        }
                    }

                    foreach (DBMessage otherElement in other.allMessages())
                    {
                        bool found = false;
                        foreach (DBMessage subElement in obj.allMessages())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Messages", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (DBMessage subElement in obj.allMessages())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Messages", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allMessages() != null)
                {
                    foreach (DBMessage otherElement in other.allMessages())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Messages", otherElement.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two SubStep and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareSubStep(SubStep obj, SubStep other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (obj.allActions() != null)
            {
                if (other.allActions() != null)
                {
                    int i = 0;
                    while (i < obj.countActions() && i < other.countActions())
                    {
                        Action element = obj.getActions(i);
                        Action otherElement = other.getActions(i);
                        compareAction(element, otherElement, diff);
                        i += 1;
                    }
                    while (i < obj.countActions())
                    {
                        diff.appendChanges(new Diff(obj.getActions(i), acceptor.ChangeOperationEnum.aAdd, "Actions", "", obj.getActions(i).Name));
                        i += 1;
                    }
                    while (i < other.countActions())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Actions", other.getActions(i).Name));
                        i += 1;
                    }
                }
                else
                {
                    foreach (Action subElement in obj.allActions())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Actions", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allActions() != null)
                {
                    foreach (Action otherElement in other.allActions())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Actions", otherElement.Name));
                    }
                }
            }
            if (obj.allExpectations() != null)
            {
                if (other.allExpectations() != null)
                {
                    int i = 0;
                    while (i < obj.countExpectations() && i < other.countExpectations())
                    {
                        Expectation element = obj.getExpectations(i);
                        Expectation otherElement = other.getExpectations(i);
                        compareExpectation(element, otherElement, diff);
                        i += 1;
                    }
                    while (i < obj.countExpectations())
                    {
                        diff.appendChanges(new Diff(obj.getExpectations(i), acceptor.ChangeOperationEnum.aAdd, "Expectations", "", obj.getExpectations(i).Name));
                        i += 1;
                    }
                    while (i < other.countExpectations())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Expectations", other.getExpectations(i).Name));
                        i += 1;
                    }
                }
                else
                {
                    foreach (Expectation subElement in obj.allExpectations())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Expectations", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allExpectations() != null)
                {
                    foreach (Expectation otherElement in other.allExpectations())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Expectations", otherElement.Name));
                    }
                }
            }
            if (obj.getSkipEngine() != other.getSkipEngine())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "SkipEngine", other.getSkipEngine().ToString(), obj.getSkipEngine().ToString()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getComment(), other.getComment()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Comment", other.getComment(), obj.getComment()));
            }
        }

        /// <summary>
        /// Compares two Expectation and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareExpectation(Expectation obj, Expectation other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getValue(), other.getValue()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Value", other.getValue(), obj.getValue()));
            }
            if (obj.getBlocking() != other.getBlocking())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Blocking", other.getBlocking().ToString(), obj.getBlocking().ToString()));
            }
            if (obj.getKind() != other.getKind())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Kind", other.getKind().ToString(), obj.getKind().ToString()));
            }
            if (obj.getDeadLine() != other.getDeadLine())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "DeadLine", other.getDeadLine().ToString(), obj.getDeadLine().ToString()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getCondition(), other.getCondition()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Condition", other.getCondition(), obj.getCondition()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getComment(), other.getComment()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Comment", other.getComment(), obj.getComment()));
            }
            if (obj.getCyclePhase() != other.getCyclePhase())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "CyclePhase", other.getCyclePhase().ToString(), obj.getCyclePhase().ToString()));
            }
        }

        /// <summary>
        /// Compares two DBMessage and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareDBMessage(DBMessage obj, DBMessage other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (obj.getMessageOrder() != other.getMessageOrder())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "MessageOrder", other.getMessageOrder().ToString(), obj.getMessageOrder().ToString()));
            }
            if (obj.getMessageType() != other.getMessageType())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "MessageType", other.getMessageType().ToString(), obj.getMessageType().ToString()));
            }
            if (obj.allFields() != null)
            {
                if (other.allFields() != null)
                {
                    foreach (DBField subElement in obj.allFields())
                    {
                        bool compared = false;
                        foreach (DBField otherElement in other.allFields())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareDBField(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Fields", "", subElement.Name));
                        }
                    }

                    foreach (DBField otherElement in other.allFields())
                    {
                        bool found = false;
                        foreach (DBField subElement in obj.allFields())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Fields", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (DBField subElement in obj.allFields())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Fields", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allFields() != null)
                {
                    foreach (DBField otherElement in other.allFields())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Fields", otherElement.Name));
                    }
                }
            }
            if (obj.allPackets() != null)
            {
                if (other.allPackets() != null)
                {
                    foreach (DBPacket subElement in obj.allPackets())
                    {
                        bool compared = false;
                        foreach (DBPacket otherElement in other.allPackets())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareDBPacket(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Packets", "", subElement.Name));
                        }
                    }

                    foreach (DBPacket otherElement in other.allPackets())
                    {
                        bool found = false;
                        foreach (DBPacket subElement in obj.allPackets())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Packets", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (DBPacket subElement in obj.allPackets())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Packets", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allPackets() != null)
                {
                    foreach (DBPacket otherElement in other.allPackets())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Packets", otherElement.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two DBPacket and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareDBPacket(DBPacket obj, DBPacket other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (obj.allFields() != null)
            {
                if (other.allFields() != null)
                {
                    foreach (DBField subElement in obj.allFields())
                    {
                        bool compared = false;
                        foreach (DBField otherElement in other.allFields())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareDBField(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Fields", "", subElement.Name));
                        }
                    }

                    foreach (DBField otherElement in other.allFields())
                    {
                        bool found = false;
                        foreach (DBField subElement in obj.allFields())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Fields", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (DBField subElement in obj.allFields())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Fields", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allFields() != null)
                {
                    foreach (DBField otherElement in other.allFields())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Fields", otherElement.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two DBField and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareDBField(DBField obj, DBField other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getVariable(), other.getVariable()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Variable", other.getVariable(), obj.getVariable()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getValue(), other.getValue()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Value", other.getValue(), obj.getValue()));
            }
        }

        /// <summary>
        /// Compares two TranslationDictionary and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareTranslationDictionary(TranslationDictionary obj, TranslationDictionary other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (obj.allFolders() != null)
            {
                if (other.allFolders() != null)
                {
                    foreach (Folder subElement in obj.allFolders())
                    {
                        bool compared = false;
                        foreach (Folder otherElement in other.allFolders())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareFolder(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Folders", "", subElement.Name));
                        }
                    }

                    foreach (Folder otherElement in other.allFolders())
                    {
                        bool found = false;
                        foreach (Folder subElement in obj.allFolders())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Folders", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Folder subElement in obj.allFolders())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Folders", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allFolders() != null)
                {
                    foreach (Folder otherElement in other.allFolders())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Folders", otherElement.Name));
                    }
                }
            }
            if (obj.allTranslations() != null)
            {
                if (other.allTranslations() != null)
                {
                    foreach (Translation subElement in obj.allTranslations())
                    {
                        bool compared = false;
                        foreach (Translation otherElement in other.allTranslations())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareTranslation(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Translations", "", subElement.Name));
                        }
                    }

                    foreach (Translation otherElement in other.allTranslations())
                    {
                        bool found = false;
                        foreach (Translation subElement in obj.allTranslations())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Translations", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Translation subElement in obj.allTranslations())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Translations", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allTranslations() != null)
                {
                    foreach (Translation otherElement in other.allTranslations())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Translations", otherElement.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two Folder and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareFolder(Folder obj, Folder other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (obj.allFolders() != null)
            {
                if (other.allFolders() != null)
                {
                    foreach (Folder subElement in obj.allFolders())
                    {
                        bool compared = false;
                        foreach (Folder otherElement in other.allFolders())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareFolder(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Folders", "", subElement.Name));
                        }
                    }

                    foreach (Folder otherElement in other.allFolders())
                    {
                        bool found = false;
                        foreach (Folder subElement in obj.allFolders())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Folders", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Folder subElement in obj.allFolders())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Folders", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allFolders() != null)
                {
                    foreach (Folder otherElement in other.allFolders())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Folders", otherElement.Name));
                    }
                }
            }
            if (obj.allTranslations() != null)
            {
                if (other.allTranslations() != null)
                {
                    foreach (Translation subElement in obj.allTranslations())
                    {
                        bool compared = false;
                        foreach (Translation otherElement in other.allTranslations())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareTranslation(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Translations", "", subElement.Name));
                        }
                    }

                    foreach (Translation otherElement in other.allTranslations())
                    {
                        bool found = false;
                        foreach (Translation subElement in obj.allTranslations())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Translations", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Translation subElement in obj.allTranslations())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Translations", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allTranslations() != null)
                {
                    foreach (Translation otherElement in other.allTranslations())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Translations", otherElement.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two Translation and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareTranslation(Translation obj, Translation other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareReferencesParagraph(obj, other, diff);

            if (obj.allSourceTexts() != null)
            {
                if (other.allSourceTexts() != null)
                {
                    foreach (SourceText subElement in obj.allSourceTexts())
                    {
                        bool compared = false;
                        foreach (SourceText otherElement in other.allSourceTexts())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareSourceText(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SourceTexts", "", subElement.Name));
                        }
                    }

                    foreach (SourceText otherElement in other.allSourceTexts())
                    {
                        bool found = false;
                        foreach (SourceText subElement in obj.allSourceTexts())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SourceTexts", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (SourceText subElement in obj.allSourceTexts())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SourceTexts", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allSourceTexts() != null)
                {
                    foreach (SourceText otherElement in other.allSourceTexts())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SourceTexts", otherElement.Name));
                    }
                }
            }
            if (obj.getImplemented() != other.getImplemented())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Implemented", other.getImplemented().ToString(), obj.getImplemented().ToString()));
            }
            if (obj.allSubSteps() != null)
            {
                if (other.allSubSteps() != null)
                {
                    foreach (SubStep subElement in obj.allSubSteps())
                    {
                        bool compared = false;
                        foreach (SubStep otherElement in other.allSubSteps())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareSubStep(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SubSteps", "", subElement.Name));
                        }
                    }

                    foreach (SubStep otherElement in other.allSubSteps())
                    {
                        bool found = false;
                        foreach (SubStep subElement in obj.allSubSteps())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SubSteps", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (SubStep subElement in obj.allSubSteps())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SubSteps", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allSubSteps() != null)
                {
                    foreach (SubStep otherElement in other.allSubSteps())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SubSteps", otherElement.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two SourceText and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareSourceText(SourceText obj, SourceText other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (obj.allComments() != null)
            {
                if (other.allComments() != null)
                {
                    foreach (SourceTextComment subElement in obj.allComments())
                    {
                        bool compared = false;
                        foreach (SourceTextComment otherElement in other.allComments())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareSourceTextComment(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Comments", "", subElement.Name));
                        }
                    }

                    foreach (SourceTextComment otherElement in other.allComments())
                    {
                        bool found = false;
                        foreach (SourceTextComment subElement in obj.allComments())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Comments", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (SourceTextComment subElement in obj.allComments())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Comments", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allComments() != null)
                {
                    foreach (SourceTextComment otherElement in other.allComments())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Comments", otherElement.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two SourceTextComment and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareSourceTextComment(SourceTextComment obj, SourceTextComment other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);
        }

        /// <summary>
        /// Compares two ShortcutDictionary and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareShortcutDictionary(ShortcutDictionary obj, ShortcutDictionary other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (obj.allFolders() != null)
            {
                if (other.allFolders() != null)
                {
                    foreach (ShortcutFolder subElement in obj.allFolders())
                    {
                        bool compared = false;
                        foreach (ShortcutFolder otherElement in other.allFolders())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareShortcutFolder(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Folders", "", subElement.Name));
                        }
                    }

                    foreach (ShortcutFolder otherElement in other.allFolders())
                    {
                        bool found = false;
                        foreach (ShortcutFolder subElement in obj.allFolders())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Folders", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (ShortcutFolder subElement in obj.allFolders())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Folders", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allFolders() != null)
                {
                    foreach (ShortcutFolder otherElement in other.allFolders())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Folders", otherElement.Name));
                    }
                }
            }
            if (obj.allShortcuts() != null)
            {
                if (other.allShortcuts() != null)
                {
                    foreach (Shortcut subElement in obj.allShortcuts())
                    {
                        bool compared = false;
                        foreach (Shortcut otherElement in other.allShortcuts())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareShortcut(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Shortcuts", "", subElement.Name));
                        }
                    }

                    foreach (Shortcut otherElement in other.allShortcuts())
                    {
                        bool found = false;
                        foreach (Shortcut subElement in obj.allShortcuts())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Shortcuts", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Shortcut subElement in obj.allShortcuts())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Shortcuts", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allShortcuts() != null)
                {
                    foreach (Shortcut otherElement in other.allShortcuts())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Shortcuts", otherElement.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two ShortcutFolder and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareShortcutFolder(ShortcutFolder obj, ShortcutFolder other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (obj.allFolders() != null)
            {
                if (other.allFolders() != null)
                {
                    foreach (ShortcutFolder subElement in obj.allFolders())
                    {
                        bool compared = false;
                        foreach (ShortcutFolder otherElement in other.allFolders())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareShortcutFolder(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Folders", "", subElement.Name));
                        }
                    }

                    foreach (ShortcutFolder otherElement in other.allFolders())
                    {
                        bool found = false;
                        foreach (ShortcutFolder subElement in obj.allFolders())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Folders", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (ShortcutFolder subElement in obj.allFolders())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Folders", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allFolders() != null)
                {
                    foreach (ShortcutFolder otherElement in other.allFolders())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Folders", otherElement.Name));
                    }
                }
            }
            if (obj.allShortcuts() != null)
            {
                if (other.allShortcuts() != null)
                {
                    foreach (Shortcut subElement in obj.allShortcuts())
                    {
                        bool compared = false;
                        foreach (Shortcut otherElement in other.allShortcuts())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareShortcut(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Shortcuts", "", subElement.Name));
                        }
                    }

                    foreach (Shortcut otherElement in other.allShortcuts())
                    {
                        bool found = false;
                        foreach (Shortcut subElement in obj.allShortcuts())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Shortcuts", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Shortcut subElement in obj.allShortcuts())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Shortcuts", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allShortcuts() != null)
                {
                    foreach (Shortcut otherElement in other.allShortcuts())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Shortcuts", otherElement.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two Shortcut and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareShortcut(Shortcut obj, Shortcut other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getShortcutName(), other.getShortcutName()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "ShortcutName", other.getShortcutName(), obj.getShortcutName()));
            }
        }

        /// <summary>
        /// Compares two RequirementSet and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareRequirementSet(RequirementSet obj, RequirementSet other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (obj.allDependancies() != null)
            {
                if (other.allDependancies() != null)
                {
                    foreach (RequirementSetDependancy subElement in obj.allDependancies())
                    {
                        bool compared = false;
                        foreach (RequirementSetDependancy otherElement in other.allDependancies())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareRequirementSetDependancy(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Dependancies", "", subElement.Name));
                        }
                    }

                    foreach (RequirementSetDependancy otherElement in other.allDependancies())
                    {
                        bool found = false;
                        foreach (RequirementSetDependancy subElement in obj.allDependancies())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Dependancies", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (RequirementSetDependancy subElement in obj.allDependancies())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Dependancies", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allDependancies() != null)
                {
                    foreach (RequirementSetDependancy otherElement in other.allDependancies())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Dependancies", otherElement.Name));
                    }
                }
            }
            if (obj.allSubSets() != null)
            {
                if (other.allSubSets() != null)
                {
                    foreach (RequirementSet subElement in obj.allSubSets())
                    {
                        bool compared = false;
                        foreach (RequirementSet otherElement in other.allSubSets())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareRequirementSet(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SubSets", "", subElement.Name));
                        }
                    }

                    foreach (RequirementSet otherElement in other.allSubSets())
                    {
                        bool found = false;
                        foreach (RequirementSet subElement in obj.allSubSets())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SubSets", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (RequirementSet subElement in obj.allSubSets())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "SubSets", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allSubSets() != null)
                {
                    foreach (RequirementSet otherElement in other.allSubSets())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "SubSets", otherElement.Name));
                    }
                }
            }
            if (obj.getWidth() != other.getWidth())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Width", other.getWidth().ToString(), obj.getWidth().ToString()));
            }
            if (obj.getHeight() != other.getHeight())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Height", other.getHeight().ToString(), obj.getHeight().ToString()));
            }
            if (obj.getX() != other.getX())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "X", other.getX().ToString(), obj.getX().ToString()));
            }
            if (obj.getY() != other.getY())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Y", other.getY().ToString(), obj.getY().ToString()));
            }
            if (obj.getRecursiveSelection() != other.getRecursiveSelection())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "RecursiveSelection", other.getRecursiveSelection().ToString(), obj.getRecursiveSelection().ToString()));
            }
            if (obj.getObsoleteRequirementsStatus() != other.getObsoleteRequirementsStatus())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "ObsoleteRequirementsStatus", other.getObsoleteRequirementsStatus().ToString(), obj.getObsoleteRequirementsStatus().ToString()));
            }
            if (obj.getDefault() != other.getDefault())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Default", other.getDefault().ToString(), obj.getDefault().ToString()));
            }
            if (obj.getPinned() != other.getPinned())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Pinned", other.getPinned().ToString(), obj.getPinned().ToString()));
            }
        }

        /// <summary>
        /// Compares two RequirementSetDependancy and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareRequirementSetDependancy(RequirementSetDependancy obj, RequirementSetDependancy other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getTarget(), other.getTarget()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Target", other.getTarget(), obj.getTarget()));
            }
        }

        /// <summary>
        /// Compares two Specification and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareSpecification(Generated.Specification obj, Generated.Specification other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getVersion(), other.getVersion()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Version", other.getVersion(), obj.getVersion()));
            }
            if (obj.allChapters() != null)
            {
                if (other.allChapters() != null)
                {
                    foreach (Chapter subElement in obj.allChapters())
                    {
                        bool compared = false;
                        foreach (Chapter otherElement in other.allChapters())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareChapter(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Chapters", "", subElement.Name));
                        }
                    }

                    foreach (Chapter otherElement in other.allChapters())
                    {
                        bool found = false;
                        foreach (Chapter subElement in obj.allChapters())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Chapters", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Chapter subElement in obj.allChapters())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Chapters", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allChapters() != null)
                {
                    foreach (Chapter otherElement in other.allChapters())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Chapters", otherElement.Name));
                    }
                }
            }
            if (obj.allChapterRefs() != null)
            {
                if (other.allChapterRefs() != null)
                {
                    foreach (ChapterRef subElement in obj.allChapterRefs())
                    {
                        bool compared = false;
                        foreach (ChapterRef otherElement in other.allChapterRefs())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareChapterRef(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "ChapterRefs", "", subElement.Name));
                        }
                    }

                    foreach (ChapterRef otherElement in other.allChapterRefs())
                    {
                        bool found = false;
                        foreach (ChapterRef subElement in obj.allChapterRefs())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "ChapterRefs", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (ChapterRef subElement in obj.allChapterRefs())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "ChapterRefs", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allChapterRefs() != null)
                {
                    foreach (ChapterRef otherElement in other.allChapterRefs())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "ChapterRefs", otherElement.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two ChapterRef and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareChapterRef(ChapterRef obj, ChapterRef other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);
        }

        /// <summary>
        /// Compares two Chapter and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareChapter(Chapter obj, Chapter other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareNamable(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getId(), other.getId()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Id", other.getId(), obj.getId()));
            }
            if (obj.allParagraphs() != null)
            {
                if (other.allParagraphs() != null)
                {
                    foreach (Paragraph subElement in obj.allParagraphs())
                    {
                        bool compared = false;
                        foreach (Paragraph otherElement in other.allParagraphs())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareParagraph(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Paragraphs", "", subElement.Name));
                        }
                    }

                    foreach (Paragraph otherElement in other.allParagraphs())
                    {
                        bool found = false;
                        foreach (Paragraph subElement in obj.allParagraphs())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Paragraphs", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Paragraph subElement in obj.allParagraphs())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Paragraphs", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allParagraphs() != null)
                {
                    foreach (Paragraph otherElement in other.allParagraphs())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Paragraphs", otherElement.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two Paragraph and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareParagraph(Paragraph obj, Paragraph other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            compareReferencesParagraph(obj, other, diff);

            if (!CompareUtil.canonicalStringEquality(obj.getId(), other.getId()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Id", other.getId(), obj.getId()));
            }
            if (obj.getType() != other.getType())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Type", other.getType().ToString(), obj.getType().ToString()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getBl(), other.getBl()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Bl", other.getBl(), obj.getBl()));
            }
            if (obj.getOptional() != other.getOptional())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Optional", other.getOptional().ToString(), obj.getOptional().ToString()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getText(), other.getText()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Text", other.getText(), obj.getText()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getVersion(), other.getVersion()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Version", other.getVersion(), obj.getVersion()));
            }
            if (obj.getReviewed() != other.getReviewed())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Reviewed", other.getReviewed().ToString(), obj.getReviewed().ToString()));
            }
            if (obj.getImplementationStatus() != other.getImplementationStatus())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "ImplementationStatus", other.getImplementationStatus().ToString(), obj.getImplementationStatus().ToString()));
            }
            if (obj.allParagraphs() != null)
            {
                if (other.allParagraphs() != null)
                {
                    foreach (Paragraph subElement in obj.allParagraphs())
                    {
                        bool compared = false;
                        foreach (Paragraph otherElement in other.allParagraphs())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareParagraph(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Paragraphs", "", subElement.Name));
                        }
                    }

                    foreach (Paragraph otherElement in other.allParagraphs())
                    {
                        bool found = false;
                        foreach (Paragraph subElement in obj.allParagraphs())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Paragraphs", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (Paragraph subElement in obj.allParagraphs())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "Paragraphs", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allParagraphs() != null)
                {
                    foreach (Paragraph otherElement in other.allParagraphs())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "Paragraphs", otherElement.Name));
                    }
                }
            }
            if (obj.getRevision() == null)
            {
                if (other.getRevision() != null)
                {
                    diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Revision", ""));
                }
            }
            else
            {
                compareParagraphRevision(obj.getRevision(), other.getRevision(), diff);
            }
            if (obj.getMoreInfoRequired() != other.getMoreInfoRequired())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "MoreInfoRequired", other.getMoreInfoRequired().ToString(), obj.getMoreInfoRequired().ToString()));
            }
            if (obj.getSpecIssue() != other.getSpecIssue())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "SpecIssue", other.getSpecIssue().ToString(), obj.getSpecIssue().ToString()));
            }
            if (obj.allRequirementSets() != null)
            {
                if (other.allRequirementSets() != null)
                {
                    foreach (RequirementSetReference subElement in obj.allRequirementSets())
                    {
                        bool compared = false;
                        foreach (RequirementSetReference otherElement in other.allRequirementSets())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                compareRequirementSetReference(subElement, otherElement, diff);
                                compared = true;
                                break;
                            }
                        }

                        if (!compared)
                        {
                            diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "RequirementSets", "", subElement.Name));
                        }
                    }

                    foreach (RequirementSetReference otherElement in other.allRequirementSets())
                    {
                        bool found = false;
                        foreach (RequirementSetReference subElement in obj.allRequirementSets())
                        {
                            if (subElement.Guid == otherElement.Guid)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "RequirementSets", otherElement.Name));
                        }
                    }
                }
                else
                {
                    foreach (RequirementSetReference subElement in obj.allRequirementSets())
                    {
                        diff.appendChanges(new Diff(subElement, acceptor.ChangeOperationEnum.aAdd, "RequirementSets", "", subElement.Name));
                    }
                }
            }
            else
            {
                if (other.allRequirementSets() != null)
                {
                    foreach (RequirementSetReference otherElement in other.allRequirementSets())
                    {
                        diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aRemove, "RequirementSets", otherElement.Name));
                    }
                }
            }
            if (obj.getTested() != other.getTested())
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Tested", other.getTested().ToString(), obj.getTested().ToString()));
            }
        }

        /// <summary>
        /// Compares two RequirementSetReference and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareRequirementSetReference(RequirementSetReference obj, RequirementSetReference other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            if (!CompareUtil.canonicalStringEquality(obj.getTarget(), other.getTarget()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Target", other.getTarget(), obj.getTarget()));
            }
        }

        /// <summary>
        /// Compares two ParagraphRevision and annotates the differences on the first one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void compareParagraphRevision(ParagraphRevision obj, ParagraphRevision other, VersionDiff diff)
        {
            if (other == null)
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aAdd, "", "", obj.Name));
                return;
            }

            if (!CompareUtil.canonicalStringEquality(obj.getText(), other.getText()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Text", other.getText(), obj.getText()));
            }
            if (!CompareUtil.canonicalStringEquality(obj.getVersion(), other.getVersion()))
            {
                diff.appendChanges(new Diff(obj, acceptor.ChangeOperationEnum.aChange, "Version", other.getVersion(), obj.getVersion()));
            }
        }

        /// <summary>
        /// Ensures that two Namable have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidNamable(Generated.Namable obj, Generated.Namable other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }
        }

        /// <summary>
        /// Ensures that two ReferencesParagraph have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidReferencesParagraph(Generated.ReferencesParagraph obj, Generated.ReferencesParagraph other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);

            if (obj.allRequirements() != null)
            {
                if (other.allRequirements() != null)
                {
                    foreach (Generated.ReqRef subElement in obj.allRequirements())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Generated.ReqRef otherElement in other.allRequirements())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidReqRef(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Generated.ReqRef otherElement in other.allRequirements())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidReqRef(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidReqRef(subElement, null);
                        }
                    }

                    foreach (Generated.ReqRef otherElement in other.allRequirements())
                    {
                        bool found = false;
                        foreach (Generated.ReqRef subElement in obj.allRequirements())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidReqRef(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Generated.ReqRef subElement in obj.allRequirements())
                    {
                        ensureGuidReqRef(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allRequirements() != null)
                {
                    foreach (Generated.ReqRef otherElement in other.allRequirements())
                    {
                        ensureGuidReqRef(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two ReqRelated have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidReqRelated(Generated.ReqRelated obj, Generated.ReqRelated other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidReferencesParagraph(obj, other);
        }

        /// <summary>
        /// Ensures that two Dictionary have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidDictionary(Generated.Dictionary obj, Generated.Dictionary other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            if (obj.allSpecifications() != null)
            {
                if (other.allSpecifications() != null)
                {
                    foreach (Generated.Specification subElement in obj.allSpecifications())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Generated.Specification otherElement in other.allSpecifications())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidSpecification(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Generated.Specification otherElement in other.allSpecifications())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidSpecification(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidSpecification(subElement, null);
                        }
                    }

                    foreach (Generated.Specification otherElement in other.allSpecifications())
                    {
                        bool found = false;
                        foreach (Generated.Specification subElement in obj.allSpecifications())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidSpecification(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Generated.Specification subElement in obj.allSpecifications())
                    {
                        ensureGuidSpecification(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allSpecifications() != null)
                {
                    foreach (Generated.Specification otherElement in other.allSpecifications())
                    {
                        ensureGuidSpecification(null, otherElement);
                    }
                }
            }
            if (obj.allRequirementSets() != null)
            {
                if (other.allRequirementSets() != null)
                {
                    foreach (RequirementSet subElement in obj.allRequirementSets())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (RequirementSet otherElement in other.allRequirementSets())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidRequirementSet(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (RequirementSet otherElement in other.allRequirementSets())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidRequirementSet(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRequirementSet(subElement, null);
                        }
                    }

                    foreach (RequirementSet otherElement in other.allRequirementSets())
                    {
                        bool found = false;
                        foreach (RequirementSet subElement in obj.allRequirementSets())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRequirementSet(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (RequirementSet subElement in obj.allRequirementSets())
                    {
                        ensureGuidRequirementSet(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allRequirementSets() != null)
                {
                    foreach (RequirementSet otherElement in other.allRequirementSets())
                    {
                        ensureGuidRequirementSet(null, otherElement);
                    }
                }
            }
            if (obj.allRuleDisablings() != null)
            {
                if (other.allRuleDisablings() != null)
                {
                    foreach (RuleDisabling subElement in obj.allRuleDisablings())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (RuleDisabling otherElement in other.allRuleDisablings())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidRuleDisabling(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (RuleDisabling otherElement in other.allRuleDisablings())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidRuleDisabling(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRuleDisabling(subElement, null);
                        }
                    }

                    foreach (RuleDisabling otherElement in other.allRuleDisablings())
                    {
                        bool found = false;
                        foreach (RuleDisabling subElement in obj.allRuleDisablings())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRuleDisabling(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (RuleDisabling subElement in obj.allRuleDisablings())
                    {
                        ensureGuidRuleDisabling(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allRuleDisablings() != null)
                {
                    foreach (RuleDisabling otherElement in other.allRuleDisablings())
                    {
                        ensureGuidRuleDisabling(null, otherElement);
                    }
                }
            }
            if (obj.allNameSpaces() != null)
            {
                if (other.allNameSpaces() != null)
                {
                    foreach (NameSpace subElement in obj.allNameSpaces())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (NameSpace otherElement in other.allNameSpaces())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidNameSpace(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (NameSpace otherElement in other.allNameSpaces())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidNameSpace(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidNameSpace(subElement, null);
                        }
                    }

                    foreach (NameSpace otherElement in other.allNameSpaces())
                    {
                        bool found = false;
                        foreach (NameSpace subElement in obj.allNameSpaces())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidNameSpace(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (NameSpace subElement in obj.allNameSpaces())
                    {
                        ensureGuidNameSpace(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allNameSpaces() != null)
                {
                    foreach (NameSpace otherElement in other.allNameSpaces())
                    {
                        ensureGuidNameSpace(null, otherElement);
                    }
                }
            }
            if (obj.allNameSpaceRefs() != null)
            {
                if (other.allNameSpaceRefs() != null)
                {
                    foreach (NameSpaceRef subElement in obj.allNameSpaceRefs())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (NameSpaceRef otherElement in other.allNameSpaceRefs())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidNameSpaceRef(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (NameSpaceRef otherElement in other.allNameSpaceRefs())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidNameSpaceRef(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidNameSpaceRef(subElement, null);
                        }
                    }

                    foreach (NameSpaceRef otherElement in other.allNameSpaceRefs())
                    {
                        bool found = false;
                        foreach (NameSpaceRef subElement in obj.allNameSpaceRefs())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidNameSpaceRef(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (NameSpaceRef subElement in obj.allNameSpaceRefs())
                    {
                        ensureGuidNameSpaceRef(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allNameSpaceRefs() != null)
                {
                    foreach (NameSpaceRef otherElement in other.allNameSpaceRefs())
                    {
                        ensureGuidNameSpaceRef(null, otherElement);
                    }
                }
            }
            if (obj.allTests() != null)
            {
                if (other.allTests() != null)
                {
                    foreach (Frame subElement in obj.allTests())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Frame otherElement in other.allTests())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidFrame(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Frame otherElement in other.allTests())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidFrame(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidFrame(subElement, null);
                        }
                    }

                    foreach (Frame otherElement in other.allTests())
                    {
                        bool found = false;
                        foreach (Frame subElement in obj.allTests())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidFrame(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Frame subElement in obj.allTests())
                    {
                        ensureGuidFrame(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allTests() != null)
                {
                    foreach (Frame otherElement in other.allTests())
                    {
                        ensureGuidFrame(null, otherElement);
                    }
                }
            }
            if (obj.allTestRefs() != null)
            {
                if (other.allTestRefs() != null)
                {
                    foreach (FrameRef subElement in obj.allTestRefs())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (FrameRef otherElement in other.allTestRefs())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidFrameRef(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (FrameRef otherElement in other.allTestRefs())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidFrameRef(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidFrameRef(subElement, null);
                        }
                    }

                    foreach (FrameRef otherElement in other.allTestRefs())
                    {
                        bool found = false;
                        foreach (FrameRef subElement in obj.allTestRefs())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidFrameRef(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (FrameRef subElement in obj.allTestRefs())
                    {
                        ensureGuidFrameRef(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allTestRefs() != null)
                {
                    foreach (FrameRef otherElement in other.allTestRefs())
                    {
                        ensureGuidFrameRef(null, otherElement);
                    }
                }
            }
            ensureGuidTranslationDictionary(obj.getTranslationDictionary(), other.getTranslationDictionary());
            ensureGuidShortcutDictionary(obj.getShortcutDictionary(), other.getShortcutDictionary());
        }

        /// <summary>
        /// Ensures that two RuleDisabling have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidRuleDisabling(RuleDisabling obj, RuleDisabling other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidReqRelated(obj, other);
        }

        /// <summary>
        /// Ensures that two NameSpaceRef have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidNameSpaceRef(NameSpaceRef obj, NameSpaceRef other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);
        }

        /// <summary>
        /// Ensures that two NameSpace have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidNameSpace(NameSpace obj, NameSpace other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);

            if (obj.allNameSpaces() != null)
            {
                if (other.allNameSpaces() != null)
                {
                    foreach (NameSpace subElement in obj.allNameSpaces())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (NameSpace otherElement in other.allNameSpaces())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidNameSpace(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (NameSpace otherElement in other.allNameSpaces())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidNameSpace(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidNameSpace(subElement, null);
                        }
                    }

                    foreach (NameSpace otherElement in other.allNameSpaces())
                    {
                        bool found = false;
                        foreach (NameSpace subElement in obj.allNameSpaces())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidNameSpace(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (NameSpace subElement in obj.allNameSpaces())
                    {
                        ensureGuidNameSpace(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allNameSpaces() != null)
                {
                    foreach (NameSpace otherElement in other.allNameSpaces())
                    {
                        ensureGuidNameSpace(null, otherElement);
                    }
                }
            }
            if (obj.allNameSpaceRefs() != null)
            {
                if (other.allNameSpaceRefs() != null)
                {
                    foreach (NameSpaceRef subElement in obj.allNameSpaceRefs())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (NameSpaceRef otherElement in other.allNameSpaceRefs())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidNameSpaceRef(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (NameSpaceRef otherElement in other.allNameSpaceRefs())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidNameSpaceRef(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidNameSpaceRef(subElement, null);
                        }
                    }

                    foreach (NameSpaceRef otherElement in other.allNameSpaceRefs())
                    {
                        bool found = false;
                        foreach (NameSpaceRef subElement in obj.allNameSpaceRefs())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidNameSpaceRef(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (NameSpaceRef subElement in obj.allNameSpaceRefs())
                    {
                        ensureGuidNameSpaceRef(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allNameSpaceRefs() != null)
                {
                    foreach (NameSpaceRef otherElement in other.allNameSpaceRefs())
                    {
                        ensureGuidNameSpaceRef(null, otherElement);
                    }
                }
            }
            if (obj.allRanges() != null)
            {
                if (other.allRanges() != null)
                {
                    foreach (Range subElement in obj.allRanges())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Range otherElement in other.allRanges())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidRange(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Range otherElement in other.allRanges())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidRange(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRange(subElement, null);
                        }
                    }

                    foreach (Range otherElement in other.allRanges())
                    {
                        bool found = false;
                        foreach (Range subElement in obj.allRanges())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRange(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Range subElement in obj.allRanges())
                    {
                        ensureGuidRange(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allRanges() != null)
                {
                    foreach (Range otherElement in other.allRanges())
                    {
                        ensureGuidRange(null, otherElement);
                    }
                }
            }
            if (obj.allEnumerations() != null)
            {
                if (other.allEnumerations() != null)
                {
                    foreach (Enum subElement in obj.allEnumerations())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Enum otherElement in other.allEnumerations())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidEnum(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Enum otherElement in other.allEnumerations())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidEnum(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidEnum(subElement, null);
                        }
                    }

                    foreach (Enum otherElement in other.allEnumerations())
                    {
                        bool found = false;
                        foreach (Enum subElement in obj.allEnumerations())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidEnum(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Enum subElement in obj.allEnumerations())
                    {
                        ensureGuidEnum(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allEnumerations() != null)
                {
                    foreach (Enum otherElement in other.allEnumerations())
                    {
                        ensureGuidEnum(null, otherElement);
                    }
                }
            }
            if (obj.allStructures() != null)
            {
                if (other.allStructures() != null)
                {
                    foreach (Structure subElement in obj.allStructures())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Structure otherElement in other.allStructures())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidStructure(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Structure otherElement in other.allStructures())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidStructure(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidStructure(subElement, null);
                        }
                    }

                    foreach (Structure otherElement in other.allStructures())
                    {
                        bool found = false;
                        foreach (Structure subElement in obj.allStructures())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidStructure(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Structure subElement in obj.allStructures())
                    {
                        ensureGuidStructure(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allStructures() != null)
                {
                    foreach (Structure otherElement in other.allStructures())
                    {
                        ensureGuidStructure(null, otherElement);
                    }
                }
            }
            if (obj.allCollections() != null)
            {
                if (other.allCollections() != null)
                {
                    foreach (Collection subElement in obj.allCollections())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Collection otherElement in other.allCollections())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidCollection(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Collection otherElement in other.allCollections())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidCollection(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidCollection(subElement, null);
                        }
                    }

                    foreach (Collection otherElement in other.allCollections())
                    {
                        bool found = false;
                        foreach (Collection subElement in obj.allCollections())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidCollection(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Collection subElement in obj.allCollections())
                    {
                        ensureGuidCollection(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allCollections() != null)
                {
                    foreach (Collection otherElement in other.allCollections())
                    {
                        ensureGuidCollection(null, otherElement);
                    }
                }
            }
            if (obj.allStateMachines() != null)
            {
                if (other.allStateMachines() != null)
                {
                    foreach (StateMachine subElement in obj.allStateMachines())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (StateMachine otherElement in other.allStateMachines())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidStateMachine(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (StateMachine otherElement in other.allStateMachines())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidStateMachine(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidStateMachine(subElement, null);
                        }
                    }

                    foreach (StateMachine otherElement in other.allStateMachines())
                    {
                        bool found = false;
                        foreach (StateMachine subElement in obj.allStateMachines())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidStateMachine(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (StateMachine subElement in obj.allStateMachines())
                    {
                        ensureGuidStateMachine(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allStateMachines() != null)
                {
                    foreach (StateMachine otherElement in other.allStateMachines())
                    {
                        ensureGuidStateMachine(null, otherElement);
                    }
                }
            }
            if (obj.allFunctions() != null)
            {
                if (other.allFunctions() != null)
                {
                    foreach (Function subElement in obj.allFunctions())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Function otherElement in other.allFunctions())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidFunction(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Function otherElement in other.allFunctions())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidFunction(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidFunction(subElement, null);
                        }
                    }

                    foreach (Function otherElement in other.allFunctions())
                    {
                        bool found = false;
                        foreach (Function subElement in obj.allFunctions())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidFunction(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Function subElement in obj.allFunctions())
                    {
                        ensureGuidFunction(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allFunctions() != null)
                {
                    foreach (Function otherElement in other.allFunctions())
                    {
                        ensureGuidFunction(null, otherElement);
                    }
                }
            }
            if (obj.allProcedures() != null)
            {
                if (other.allProcedures() != null)
                {
                    foreach (Procedure subElement in obj.allProcedures())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Procedure otherElement in other.allProcedures())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidProcedure(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Procedure otherElement in other.allProcedures())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidProcedure(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidProcedure(subElement, null);
                        }
                    }

                    foreach (Procedure otherElement in other.allProcedures())
                    {
                        bool found = false;
                        foreach (Procedure subElement in obj.allProcedures())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidProcedure(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Procedure subElement in obj.allProcedures())
                    {
                        ensureGuidProcedure(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allProcedures() != null)
                {
                    foreach (Procedure otherElement in other.allProcedures())
                    {
                        ensureGuidProcedure(null, otherElement);
                    }
                }
            }
            if (obj.allVariables() != null)
            {
                if (other.allVariables() != null)
                {
                    foreach (Variable subElement in obj.allVariables())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Variable otherElement in other.allVariables())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidVariable(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Variable otherElement in other.allVariables())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidVariable(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidVariable(subElement, null);
                        }
                    }

                    foreach (Variable otherElement in other.allVariables())
                    {
                        bool found = false;
                        foreach (Variable subElement in obj.allVariables())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidVariable(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Variable subElement in obj.allVariables())
                    {
                        ensureGuidVariable(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allVariables() != null)
                {
                    foreach (Variable otherElement in other.allVariables())
                    {
                        ensureGuidVariable(null, otherElement);
                    }
                }
            }
            if (obj.allRules() != null)
            {
                if (other.allRules() != null)
                {
                    foreach (Rule subElement in obj.allRules())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Rule otherElement in other.allRules())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidRule(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Rule otherElement in other.allRules())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidRule(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRule(subElement, null);
                        }
                    }

                    foreach (Rule otherElement in other.allRules())
                    {
                        bool found = false;
                        foreach (Rule subElement in obj.allRules())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRule(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Rule subElement in obj.allRules())
                    {
                        ensureGuidRule(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allRules() != null)
                {
                    foreach (Rule otherElement in other.allRules())
                    {
                        ensureGuidRule(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two ReqRef have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidReqRef(Generated.ReqRef obj, Generated.ReqRef other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }
        }

        /// <summary>
        /// Ensures that two Type have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidType(Type obj, Type other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidReqRelated(obj, other);
        }

        /// <summary>
        /// Ensures that two Enum have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidEnum(Enum obj, Enum other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidType(obj, other);

            if (obj.allValues() != null)
            {
                if (other.allValues() != null)
                {
                    int i = 0;
                    while (i < obj.countValues() && i < other.countValues())
                    {
                        EnumValue element = obj.getValues(i);
                        EnumValue otherElement = other.getValues(i);
                        ensureGuidEnumValue(element, otherElement);
                        i += 1;
                    }
                    while (i < obj.countValues())
                    {
                        EnumValue element = obj.getValues(i);
                        ensureGuidEnumValue(element, null);
                        i += 1;
                    }
                    while (i < other.countValues())
                    {
                        EnumValue otherElement = other.getValues(i);
                        ensureGuidEnumValue(null, otherElement);
                        i += 1;
                    }
                }
                else
                {
                    foreach (EnumValue subElement in obj.allValues())
                    {
                        ensureGuidEnumValue(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allValues() != null)
                {
                    foreach (EnumValue otherElement in other.allValues())
                    {
                        ensureGuidEnumValue(null, otherElement);
                    }
                }
            }
            if (obj.allSubEnums() != null)
            {
                if (other.allSubEnums() != null)
                {
                    foreach (Enum subElement in obj.allSubEnums())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Enum otherElement in other.allSubEnums())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidEnum(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Enum otherElement in other.allSubEnums())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidEnum(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidEnum(subElement, null);
                        }
                    }

                    foreach (Enum otherElement in other.allSubEnums())
                    {
                        bool found = false;
                        foreach (Enum subElement in obj.allSubEnums())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidEnum(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Enum subElement in obj.allSubEnums())
                    {
                        ensureGuidEnum(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allSubEnums() != null)
                {
                    foreach (Enum otherElement in other.allSubEnums())
                    {
                        ensureGuidEnum(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two EnumValue have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidEnumValue(EnumValue obj, EnumValue other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);
        }

        /// <summary>
        /// Ensures that two Range have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidRange(Range obj, Range other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidType(obj, other);

            if (obj.allSpecialValues() != null)
            {
                if (other.allSpecialValues() != null)
                {
                    int i = 0;
                    while (i < obj.countSpecialValues() && i < other.countSpecialValues())
                    {
                        EnumValue element = obj.getSpecialValues(i);
                        EnumValue otherElement = other.getSpecialValues(i);
                        ensureGuidEnumValue(element, otherElement);
                        i += 1;
                    }
                    while (i < obj.countSpecialValues())
                    {
                        EnumValue element = obj.getSpecialValues(i);
                        ensureGuidEnumValue(element, null);
                        i += 1;
                    }
                    while (i < other.countSpecialValues())
                    {
                        EnumValue otherElement = other.getSpecialValues(i);
                        ensureGuidEnumValue(null, otherElement);
                        i += 1;
                    }
                }
                else
                {
                    foreach (EnumValue subElement in obj.allSpecialValues())
                    {
                        ensureGuidEnumValue(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allSpecialValues() != null)
                {
                    foreach (EnumValue otherElement in other.allSpecialValues())
                    {
                        ensureGuidEnumValue(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two Structure have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidStructure(Structure obj, Structure other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidType(obj, other);

            if (obj.allElements() != null)
            {
                if (other.allElements() != null)
                {
                    foreach (StructureElement subElement in obj.allElements())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (StructureElement otherElement in other.allElements())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidStructureElement(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (StructureElement otherElement in other.allElements())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidStructureElement(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidStructureElement(subElement, null);
                        }
                    }

                    foreach (StructureElement otherElement in other.allElements())
                    {
                        bool found = false;
                        foreach (StructureElement subElement in obj.allElements())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidStructureElement(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (StructureElement subElement in obj.allElements())
                    {
                        ensureGuidStructureElement(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allElements() != null)
                {
                    foreach (StructureElement otherElement in other.allElements())
                    {
                        ensureGuidStructureElement(null, otherElement);
                    }
                }
            }
            if (obj.allProcedures() != null)
            {
                if (other.allProcedures() != null)
                {
                    foreach (Procedure subElement in obj.allProcedures())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Procedure otherElement in other.allProcedures())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidProcedure(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Procedure otherElement in other.allProcedures())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidProcedure(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidProcedure(subElement, null);
                        }
                    }

                    foreach (Procedure otherElement in other.allProcedures())
                    {
                        bool found = false;
                        foreach (Procedure subElement in obj.allProcedures())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidProcedure(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Procedure subElement in obj.allProcedures())
                    {
                        ensureGuidProcedure(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allProcedures() != null)
                {
                    foreach (Procedure otherElement in other.allProcedures())
                    {
                        ensureGuidProcedure(null, otherElement);
                    }
                }
            }
            if (obj.allStateMachines() != null)
            {
                if (other.allStateMachines() != null)
                {
                    foreach (StateMachine subElement in obj.allStateMachines())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (StateMachine otherElement in other.allStateMachines())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidStateMachine(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (StateMachine otherElement in other.allStateMachines())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidStateMachine(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidStateMachine(subElement, null);
                        }
                    }

                    foreach (StateMachine otherElement in other.allStateMachines())
                    {
                        bool found = false;
                        foreach (StateMachine subElement in obj.allStateMachines())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidStateMachine(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (StateMachine subElement in obj.allStateMachines())
                    {
                        ensureGuidStateMachine(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allStateMachines() != null)
                {
                    foreach (StateMachine otherElement in other.allStateMachines())
                    {
                        ensureGuidStateMachine(null, otherElement);
                    }
                }
            }
            if (obj.allRules() != null)
            {
                if (other.allRules() != null)
                {
                    foreach (Rule subElement in obj.allRules())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Rule otherElement in other.allRules())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidRule(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Rule otherElement in other.allRules())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidRule(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRule(subElement, null);
                        }
                    }

                    foreach (Rule otherElement in other.allRules())
                    {
                        bool found = false;
                        foreach (Rule subElement in obj.allRules())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRule(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Rule subElement in obj.allRules())
                    {
                        ensureGuidRule(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allRules() != null)
                {
                    foreach (Rule otherElement in other.allRules())
                    {
                        ensureGuidRule(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two StructureElement have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidStructureElement(StructureElement obj, StructureElement other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidReqRelated(obj, other);
        }

        /// <summary>
        /// Ensures that two Collection have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidCollection(Collection obj, Collection other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidType(obj, other);
        }

        /// <summary>
        /// Ensures that two Function have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidFunction(Function obj, Function other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidType(obj, other);

            if (obj.allParameters() != null)
            {
                if (other.allParameters() != null)
                {
                    foreach (Generated.Parameter subElement in obj.allParameters())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Generated.Parameter otherElement in other.allParameters())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidParameter(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Generated.Parameter otherElement in other.allParameters())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidParameter(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidParameter(subElement, null);
                        }
                    }

                    foreach (Generated.Parameter otherElement in other.allParameters())
                    {
                        bool found = false;
                        foreach (Generated.Parameter subElement in obj.allParameters())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidParameter(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Generated.Parameter subElement in obj.allParameters())
                    {
                        ensureGuidParameter(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allParameters() != null)
                {
                    foreach (Generated.Parameter otherElement in other.allParameters())
                    {
                        ensureGuidParameter(null, otherElement);
                    }
                }
            }
            if (obj.allCases() != null)
            {
                if (other.allCases() != null)
                {
                    int i = 0;
                    while (i < obj.countCases() && i < other.countCases())
                    {
                        Case element = obj.getCases(i);
                        Case otherElement = other.getCases(i);
                        ensureGuidCase(element, otherElement);
                        i += 1;
                    }
                    while (i < obj.countCases())
                    {
                        Case element = obj.getCases(i);
                        ensureGuidCase(element, null);
                        i += 1;
                    }
                    while (i < other.countCases())
                    {
                        Case otherElement = other.getCases(i);
                        ensureGuidCase(null, otherElement);
                        i += 1;
                    }
                }
                else
                {
                    foreach (Case subElement in obj.allCases())
                    {
                        ensureGuidCase(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allCases() != null)
                {
                    foreach (Case otherElement in other.allCases())
                    {
                        ensureGuidCase(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two Parameter have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidParameter(Generated.Parameter obj, Generated.Parameter other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);
        }

        /// <summary>
        /// Ensures that two Case have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidCase(Case obj, Case other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);

            if (obj.allPreConditions() != null)
            {
                if (other.allPreConditions() != null)
                {
                    int i = 0;
                    while (i < obj.countPreConditions() && i < other.countPreConditions())
                    {
                        PreCondition element = obj.getPreConditions(i);
                        PreCondition otherElement = other.getPreConditions(i);
                        ensureGuidPreCondition(element, otherElement);
                        i += 1;
                    }
                    while (i < obj.countPreConditions())
                    {
                        PreCondition element = obj.getPreConditions(i);
                        ensureGuidPreCondition(element, null);
                        i += 1;
                    }
                    while (i < other.countPreConditions())
                    {
                        PreCondition otherElement = other.getPreConditions(i);
                        ensureGuidPreCondition(null, otherElement);
                        i += 1;
                    }
                }
                else
                {
                    foreach (PreCondition subElement in obj.allPreConditions())
                    {
                        ensureGuidPreCondition(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allPreConditions() != null)
                {
                    foreach (PreCondition otherElement in other.allPreConditions())
                    {
                        ensureGuidPreCondition(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two Procedure have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidProcedure(Procedure obj, Procedure other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidReqRelated(obj, other);

            ensureGuidStateMachine(obj.getStateMachine(), other.getStateMachine());
            if (obj.allRules() != null)
            {
                if (other.allRules() != null)
                {
                    foreach (Rule subElement in obj.allRules())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Rule otherElement in other.allRules())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidRule(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Rule otherElement in other.allRules())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidRule(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRule(subElement, null);
                        }
                    }

                    foreach (Rule otherElement in other.allRules())
                    {
                        bool found = false;
                        foreach (Rule subElement in obj.allRules())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRule(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Rule subElement in obj.allRules())
                    {
                        ensureGuidRule(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allRules() != null)
                {
                    foreach (Rule otherElement in other.allRules())
                    {
                        ensureGuidRule(null, otherElement);
                    }
                }
            }
            if (obj.allParameters() != null)
            {
                if (other.allParameters() != null)
                {
                    foreach (Generated.Parameter subElement in obj.allParameters())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Generated.Parameter otherElement in other.allParameters())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidParameter(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Generated.Parameter otherElement in other.allParameters())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidParameter(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidParameter(subElement, null);
                        }
                    }

                    foreach (Generated.Parameter otherElement in other.allParameters())
                    {
                        bool found = false;
                        foreach (Generated.Parameter subElement in obj.allParameters())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidParameter(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Generated.Parameter subElement in obj.allParameters())
                    {
                        ensureGuidParameter(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allParameters() != null)
                {
                    foreach (Generated.Parameter otherElement in other.allParameters())
                    {
                        ensureGuidParameter(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two StateMachine have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidStateMachine(StateMachine obj, StateMachine other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidType(obj, other);

            if (obj.allStates() != null)
            {
                if (other.allStates() != null)
                {
                    foreach (State subElement in obj.allStates())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (State otherElement in other.allStates())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidState(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (State otherElement in other.allStates())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidState(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidState(subElement, null);
                        }
                    }

                    foreach (State otherElement in other.allStates())
                    {
                        bool found = false;
                        foreach (State subElement in obj.allStates())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidState(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (State subElement in obj.allStates())
                    {
                        ensureGuidState(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allStates() != null)
                {
                    foreach (State otherElement in other.allStates())
                    {
                        ensureGuidState(null, otherElement);
                    }
                }
            }
            if (obj.allRules() != null)
            {
                if (other.allRules() != null)
                {
                    foreach (Rule subElement in obj.allRules())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Rule otherElement in other.allRules())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidRule(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Rule otherElement in other.allRules())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidRule(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRule(subElement, null);
                        }
                    }

                    foreach (Rule otherElement in other.allRules())
                    {
                        bool found = false;
                        foreach (Rule subElement in obj.allRules())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRule(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Rule subElement in obj.allRules())
                    {
                        ensureGuidRule(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allRules() != null)
                {
                    foreach (Rule otherElement in other.allRules())
                    {
                        ensureGuidRule(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two State have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidState(State obj, State other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidReqRelated(obj, other);

            ensureGuidStateMachine(obj.getStateMachine(), other.getStateMachine());
            ensureGuidRule(obj.getEnterAction(), other.getEnterAction());
            ensureGuidRule(obj.getLeaveAction(), other.getLeaveAction());
        }

        /// <summary>
        /// Ensures that two Variable have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidVariable(Variable obj, Variable other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidReqRelated(obj, other);

            if (obj.allSubVariables() != null)
            {
                if (other.allSubVariables() != null)
                {
                    foreach (Variable subElement in obj.allSubVariables())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Variable otherElement in other.allSubVariables())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidVariable(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Variable otherElement in other.allSubVariables())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidVariable(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidVariable(subElement, null);
                        }
                    }

                    foreach (Variable otherElement in other.allSubVariables())
                    {
                        bool found = false;
                        foreach (Variable subElement in obj.allSubVariables())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidVariable(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Variable subElement in obj.allSubVariables())
                    {
                        ensureGuidVariable(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allSubVariables() != null)
                {
                    foreach (Variable otherElement in other.allSubVariables())
                    {
                        ensureGuidVariable(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two Rule have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidRule(Rule obj, Rule other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidReqRelated(obj, other);

            if (obj.allConditions() != null)
            {
                if (other.allConditions() != null)
                {
                    foreach (RuleCondition subElement in obj.allConditions())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (RuleCondition otherElement in other.allConditions())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidRuleCondition(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (RuleCondition otherElement in other.allConditions())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidRuleCondition(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRuleCondition(subElement, null);
                        }
                    }

                    foreach (RuleCondition otherElement in other.allConditions())
                    {
                        bool found = false;
                        foreach (RuleCondition subElement in obj.allConditions())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRuleCondition(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (RuleCondition subElement in obj.allConditions())
                    {
                        ensureGuidRuleCondition(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allConditions() != null)
                {
                    foreach (RuleCondition otherElement in other.allConditions())
                    {
                        ensureGuidRuleCondition(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two RuleCondition have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidRuleCondition(RuleCondition obj, RuleCondition other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidReqRelated(obj, other);

            if (obj.allPreConditions() != null)
            {
                if (other.allPreConditions() != null)
                {
                    int i = 0;
                    while (i < obj.countPreConditions() && i < other.countPreConditions())
                    {
                        PreCondition element = obj.getPreConditions(i);
                        PreCondition otherElement = other.getPreConditions(i);
                        ensureGuidPreCondition(element, otherElement);
                        i += 1;
                    }
                    while (i < obj.countPreConditions())
                    {
                        PreCondition element = obj.getPreConditions(i);
                        ensureGuidPreCondition(element, null);
                        i += 1;
                    }
                    while (i < other.countPreConditions())
                    {
                        PreCondition otherElement = other.getPreConditions(i);
                        ensureGuidPreCondition(null, otherElement);
                        i += 1;
                    }
                }
                else
                {
                    foreach (PreCondition subElement in obj.allPreConditions())
                    {
                        ensureGuidPreCondition(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allPreConditions() != null)
                {
                    foreach (PreCondition otherElement in other.allPreConditions())
                    {
                        ensureGuidPreCondition(null, otherElement);
                    }
                }
            }
            if (obj.allActions() != null)
            {
                if (other.allActions() != null)
                {
                    int i = 0;
                    while (i < obj.countActions() && i < other.countActions())
                    {
                        Action element = obj.getActions(i);
                        Action otherElement = other.getActions(i);
                        ensureGuidAction(element, otherElement);
                        i += 1;
                    }
                    while (i < obj.countActions())
                    {
                        Action element = obj.getActions(i);
                        ensureGuidAction(element, null);
                        i += 1;
                    }
                    while (i < other.countActions())
                    {
                        Action otherElement = other.getActions(i);
                        ensureGuidAction(null, otherElement);
                        i += 1;
                    }
                }
                else
                {
                    foreach (Action subElement in obj.allActions())
                    {
                        ensureGuidAction(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allActions() != null)
                {
                    foreach (Action otherElement in other.allActions())
                    {
                        ensureGuidAction(null, otherElement);
                    }
                }
            }
            if (obj.allSubRules() != null)
            {
                if (other.allSubRules() != null)
                {
                    foreach (Rule subElement in obj.allSubRules())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Rule otherElement in other.allSubRules())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidRule(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Rule otherElement in other.allSubRules())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidRule(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRule(subElement, null);
                        }
                    }

                    foreach (Rule otherElement in other.allSubRules())
                    {
                        bool found = false;
                        foreach (Rule subElement in obj.allSubRules())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRule(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Rule subElement in obj.allSubRules())
                    {
                        ensureGuidRule(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allSubRules() != null)
                {
                    foreach (Rule otherElement in other.allSubRules())
                    {
                        ensureGuidRule(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two PreCondition have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidPreCondition(PreCondition obj, PreCondition other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }
        }

        /// <summary>
        /// Ensures that two Action have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidAction(Action obj, Action other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }
        }

        /// <summary>
        /// Ensures that two FrameRef have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidFrameRef(FrameRef obj, FrameRef other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);
        }

        /// <summary>
        /// Ensures that two Frame have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidFrame(Frame obj, Frame other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);

            if (obj.allSubSequences() != null)
            {
                if (other.allSubSequences() != null)
                {
                    foreach (SubSequence subElement in obj.allSubSequences())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (SubSequence otherElement in other.allSubSequences())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidSubSequence(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (SubSequence otherElement in other.allSubSequences())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidSubSequence(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidSubSequence(subElement, null);
                        }
                    }

                    foreach (SubSequence otherElement in other.allSubSequences())
                    {
                        bool found = false;
                        foreach (SubSequence subElement in obj.allSubSequences())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidSubSequence(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (SubSequence subElement in obj.allSubSequences())
                    {
                        ensureGuidSubSequence(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allSubSequences() != null)
                {
                    foreach (SubSequence otherElement in other.allSubSequences())
                    {
                        ensureGuidSubSequence(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two SubSequence have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidSubSequence(SubSequence obj, SubSequence other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);

            if (obj.allTestCases() != null)
            {
                if (other.allTestCases() != null)
                {
                    foreach (TestCase subElement in obj.allTestCases())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (TestCase otherElement in other.allTestCases())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidTestCase(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (TestCase otherElement in other.allTestCases())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidTestCase(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidTestCase(subElement, null);
                        }
                    }

                    foreach (TestCase otherElement in other.allTestCases())
                    {
                        bool found = false;
                        foreach (TestCase subElement in obj.allTestCases())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidTestCase(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (TestCase subElement in obj.allTestCases())
                    {
                        ensureGuidTestCase(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allTestCases() != null)
                {
                    foreach (TestCase otherElement in other.allTestCases())
                    {
                        ensureGuidTestCase(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two TestCase have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidTestCase(TestCase obj, TestCase other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidReqRelated(obj, other);

            if (obj.allSteps() != null)
            {
                if (other.allSteps() != null)
                {
                    foreach (Step subElement in obj.allSteps())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Step otherElement in other.allSteps())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidStep(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Step otherElement in other.allSteps())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidStep(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidStep(subElement, null);
                        }
                    }

                    foreach (Step otherElement in other.allSteps())
                    {
                        bool found = false;
                        foreach (Step subElement in obj.allSteps())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidStep(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Step subElement in obj.allSteps())
                    {
                        ensureGuidStep(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allSteps() != null)
                {
                    foreach (Step otherElement in other.allSteps())
                    {
                        ensureGuidStep(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two Step have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidStep(Step obj, Step other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidReferencesParagraph(obj, other);

            if (obj.allSubSteps() != null)
            {
                if (other.allSubSteps() != null)
                {
                    foreach (SubStep subElement in obj.allSubSteps())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (SubStep otherElement in other.allSubSteps())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidSubStep(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (SubStep otherElement in other.allSubSteps())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidSubStep(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidSubStep(subElement, null);
                        }
                    }

                    foreach (SubStep otherElement in other.allSubSteps())
                    {
                        bool found = false;
                        foreach (SubStep subElement in obj.allSubSteps())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidSubStep(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (SubStep subElement in obj.allSubSteps())
                    {
                        ensureGuidSubStep(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allSubSteps() != null)
                {
                    foreach (SubStep otherElement in other.allSubSteps())
                    {
                        ensureGuidSubStep(null, otherElement);
                    }
                }
            }
            if (obj.allMessages() != null)
            {
                if (other.allMessages() != null)
                {
                    foreach (DBMessage subElement in obj.allMessages())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (DBMessage otherElement in other.allMessages())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidDBMessage(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (DBMessage otherElement in other.allMessages())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidDBMessage(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidDBMessage(subElement, null);
                        }
                    }

                    foreach (DBMessage otherElement in other.allMessages())
                    {
                        bool found = false;
                        foreach (DBMessage subElement in obj.allMessages())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidDBMessage(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (DBMessage subElement in obj.allMessages())
                    {
                        ensureGuidDBMessage(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allMessages() != null)
                {
                    foreach (DBMessage otherElement in other.allMessages())
                    {
                        ensureGuidDBMessage(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two SubStep have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidSubStep(SubStep obj, SubStep other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);

            if (obj.allActions() != null)
            {
                if (other.allActions() != null)
                {
                    int i = 0;
                    while (i < obj.countActions() && i < other.countActions())
                    {
                        Action element = obj.getActions(i);
                        Action otherElement = other.getActions(i);
                        ensureGuidAction(element, otherElement);
                        i += 1;
                    }
                    while (i < obj.countActions())
                    {
                        Action element = obj.getActions(i);
                        ensureGuidAction(element, null);
                        i += 1;
                    }
                    while (i < other.countActions())
                    {
                        Action otherElement = other.getActions(i);
                        ensureGuidAction(null, otherElement);
                        i += 1;
                    }
                }
                else
                {
                    foreach (Action subElement in obj.allActions())
                    {
                        ensureGuidAction(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allActions() != null)
                {
                    foreach (Action otherElement in other.allActions())
                    {
                        ensureGuidAction(null, otherElement);
                    }
                }
            }
            if (obj.allExpectations() != null)
            {
                if (other.allExpectations() != null)
                {
                    int i = 0;
                    while (i < obj.countExpectations() && i < other.countExpectations())
                    {
                        Expectation element = obj.getExpectations(i);
                        Expectation otherElement = other.getExpectations(i);
                        ensureGuidExpectation(element, otherElement);
                        i += 1;
                    }
                    while (i < obj.countExpectations())
                    {
                        Expectation element = obj.getExpectations(i);
                        ensureGuidExpectation(element, null);
                        i += 1;
                    }
                    while (i < other.countExpectations())
                    {
                        Expectation otherElement = other.getExpectations(i);
                        ensureGuidExpectation(null, otherElement);
                        i += 1;
                    }
                }
                else
                {
                    foreach (Expectation subElement in obj.allExpectations())
                    {
                        ensureGuidExpectation(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allExpectations() != null)
                {
                    foreach (Expectation otherElement in other.allExpectations())
                    {
                        ensureGuidExpectation(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two Expectation have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidExpectation(Expectation obj, Expectation other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);
        }

        /// <summary>
        /// Ensures that two DBMessage have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidDBMessage(DBMessage obj, DBMessage other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);

            if (obj.allFields() != null)
            {
                if (other.allFields() != null)
                {
                    foreach (DBField subElement in obj.allFields())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (DBField otherElement in other.allFields())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidDBField(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (DBField otherElement in other.allFields())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidDBField(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidDBField(subElement, null);
                        }
                    }

                    foreach (DBField otherElement in other.allFields())
                    {
                        bool found = false;
                        foreach (DBField subElement in obj.allFields())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidDBField(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (DBField subElement in obj.allFields())
                    {
                        ensureGuidDBField(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allFields() != null)
                {
                    foreach (DBField otherElement in other.allFields())
                    {
                        ensureGuidDBField(null, otherElement);
                    }
                }
            }
            if (obj.allPackets() != null)
            {
                if (other.allPackets() != null)
                {
                    foreach (DBPacket subElement in obj.allPackets())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (DBPacket otherElement in other.allPackets())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidDBPacket(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (DBPacket otherElement in other.allPackets())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidDBPacket(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidDBPacket(subElement, null);
                        }
                    }

                    foreach (DBPacket otherElement in other.allPackets())
                    {
                        bool found = false;
                        foreach (DBPacket subElement in obj.allPackets())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidDBPacket(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (DBPacket subElement in obj.allPackets())
                    {
                        ensureGuidDBPacket(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allPackets() != null)
                {
                    foreach (DBPacket otherElement in other.allPackets())
                    {
                        ensureGuidDBPacket(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two DBPacket have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidDBPacket(DBPacket obj, DBPacket other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);

            if (obj.allFields() != null)
            {
                if (other.allFields() != null)
                {
                    foreach (DBField subElement in obj.allFields())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (DBField otherElement in other.allFields())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidDBField(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (DBField otherElement in other.allFields())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidDBField(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidDBField(subElement, null);
                        }
                    }

                    foreach (DBField otherElement in other.allFields())
                    {
                        bool found = false;
                        foreach (DBField subElement in obj.allFields())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidDBField(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (DBField subElement in obj.allFields())
                    {
                        ensureGuidDBField(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allFields() != null)
                {
                    foreach (DBField otherElement in other.allFields())
                    {
                        ensureGuidDBField(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two DBField have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidDBField(DBField obj, DBField other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);
        }

        /// <summary>
        /// Ensures that two TranslationDictionary have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidTranslationDictionary(TranslationDictionary obj, TranslationDictionary other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);

            if (obj.allFolders() != null)
            {
                if (other.allFolders() != null)
                {
                    foreach (Folder subElement in obj.allFolders())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Folder otherElement in other.allFolders())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidFolder(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Folder otherElement in other.allFolders())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidFolder(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidFolder(subElement, null);
                        }
                    }

                    foreach (Folder otherElement in other.allFolders())
                    {
                        bool found = false;
                        foreach (Folder subElement in obj.allFolders())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidFolder(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Folder subElement in obj.allFolders())
                    {
                        ensureGuidFolder(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allFolders() != null)
                {
                    foreach (Folder otherElement in other.allFolders())
                    {
                        ensureGuidFolder(null, otherElement);
                    }
                }
            }
            if (obj.allTranslations() != null)
            {
                if (other.allTranslations() != null)
                {
                    foreach (Translation subElement in obj.allTranslations())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Translation otherElement in other.allTranslations())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidTranslation(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Translation otherElement in other.allTranslations())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidTranslation(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidTranslation(subElement, null);
                        }
                    }

                    foreach (Translation otherElement in other.allTranslations())
                    {
                        bool found = false;
                        foreach (Translation subElement in obj.allTranslations())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidTranslation(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Translation subElement in obj.allTranslations())
                    {
                        ensureGuidTranslation(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allTranslations() != null)
                {
                    foreach (Translation otherElement in other.allTranslations())
                    {
                        ensureGuidTranslation(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two Folder have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidFolder(Folder obj, Folder other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);

            if (obj.allFolders() != null)
            {
                if (other.allFolders() != null)
                {
                    foreach (Folder subElement in obj.allFolders())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Folder otherElement in other.allFolders())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidFolder(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Folder otherElement in other.allFolders())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidFolder(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidFolder(subElement, null);
                        }
                    }

                    foreach (Folder otherElement in other.allFolders())
                    {
                        bool found = false;
                        foreach (Folder subElement in obj.allFolders())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidFolder(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Folder subElement in obj.allFolders())
                    {
                        ensureGuidFolder(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allFolders() != null)
                {
                    foreach (Folder otherElement in other.allFolders())
                    {
                        ensureGuidFolder(null, otherElement);
                    }
                }
            }
            if (obj.allTranslations() != null)
            {
                if (other.allTranslations() != null)
                {
                    foreach (Translation subElement in obj.allTranslations())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Translation otherElement in other.allTranslations())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidTranslation(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Translation otherElement in other.allTranslations())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidTranslation(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidTranslation(subElement, null);
                        }
                    }

                    foreach (Translation otherElement in other.allTranslations())
                    {
                        bool found = false;
                        foreach (Translation subElement in obj.allTranslations())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidTranslation(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Translation subElement in obj.allTranslations())
                    {
                        ensureGuidTranslation(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allTranslations() != null)
                {
                    foreach (Translation otherElement in other.allTranslations())
                    {
                        ensureGuidTranslation(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two Translation have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidTranslation(Translation obj, Translation other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidReferencesParagraph(obj, other);

            if (obj.allSourceTexts() != null)
            {
                if (other.allSourceTexts() != null)
                {
                    foreach (SourceText subElement in obj.allSourceTexts())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (SourceText otherElement in other.allSourceTexts())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidSourceText(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (SourceText otherElement in other.allSourceTexts())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidSourceText(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidSourceText(subElement, null);
                        }
                    }

                    foreach (SourceText otherElement in other.allSourceTexts())
                    {
                        bool found = false;
                        foreach (SourceText subElement in obj.allSourceTexts())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidSourceText(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (SourceText subElement in obj.allSourceTexts())
                    {
                        ensureGuidSourceText(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allSourceTexts() != null)
                {
                    foreach (SourceText otherElement in other.allSourceTexts())
                    {
                        ensureGuidSourceText(null, otherElement);
                    }
                }
            }
            if (obj.allSubSteps() != null)
            {
                if (other.allSubSteps() != null)
                {
                    foreach (SubStep subElement in obj.allSubSteps())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (SubStep otherElement in other.allSubSteps())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidSubStep(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (SubStep otherElement in other.allSubSteps())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidSubStep(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidSubStep(subElement, null);
                        }
                    }

                    foreach (SubStep otherElement in other.allSubSteps())
                    {
                        bool found = false;
                        foreach (SubStep subElement in obj.allSubSteps())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidSubStep(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (SubStep subElement in obj.allSubSteps())
                    {
                        ensureGuidSubStep(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allSubSteps() != null)
                {
                    foreach (SubStep otherElement in other.allSubSteps())
                    {
                        ensureGuidSubStep(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two SourceText have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidSourceText(SourceText obj, SourceText other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);

            if (obj.allComments() != null)
            {
                if (other.allComments() != null)
                {
                    foreach (SourceTextComment subElement in obj.allComments())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (SourceTextComment otherElement in other.allComments())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidSourceTextComment(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (SourceTextComment otherElement in other.allComments())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidSourceTextComment(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidSourceTextComment(subElement, null);
                        }
                    }

                    foreach (SourceTextComment otherElement in other.allComments())
                    {
                        bool found = false;
                        foreach (SourceTextComment subElement in obj.allComments())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidSourceTextComment(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (SourceTextComment subElement in obj.allComments())
                    {
                        ensureGuidSourceTextComment(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allComments() != null)
                {
                    foreach (SourceTextComment otherElement in other.allComments())
                    {
                        ensureGuidSourceTextComment(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two SourceTextComment have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidSourceTextComment(SourceTextComment obj, SourceTextComment other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);
        }

        /// <summary>
        /// Ensures that two ShortcutDictionary have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidShortcutDictionary(ShortcutDictionary obj, ShortcutDictionary other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);

            if (obj.allFolders() != null)
            {
                if (other.allFolders() != null)
                {
                    foreach (ShortcutFolder subElement in obj.allFolders())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (ShortcutFolder otherElement in other.allFolders())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidShortcutFolder(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (ShortcutFolder otherElement in other.allFolders())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidShortcutFolder(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidShortcutFolder(subElement, null);
                        }
                    }

                    foreach (ShortcutFolder otherElement in other.allFolders())
                    {
                        bool found = false;
                        foreach (ShortcutFolder subElement in obj.allFolders())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidShortcutFolder(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (ShortcutFolder subElement in obj.allFolders())
                    {
                        ensureGuidShortcutFolder(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allFolders() != null)
                {
                    foreach (ShortcutFolder otherElement in other.allFolders())
                    {
                        ensureGuidShortcutFolder(null, otherElement);
                    }
                }
            }
            if (obj.allShortcuts() != null)
            {
                if (other.allShortcuts() != null)
                {
                    foreach (Shortcut subElement in obj.allShortcuts())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Shortcut otherElement in other.allShortcuts())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidShortcut(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Shortcut otherElement in other.allShortcuts())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidShortcut(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidShortcut(subElement, null);
                        }
                    }

                    foreach (Shortcut otherElement in other.allShortcuts())
                    {
                        bool found = false;
                        foreach (Shortcut subElement in obj.allShortcuts())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidShortcut(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Shortcut subElement in obj.allShortcuts())
                    {
                        ensureGuidShortcut(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allShortcuts() != null)
                {
                    foreach (Shortcut otherElement in other.allShortcuts())
                    {
                        ensureGuidShortcut(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two ShortcutFolder have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidShortcutFolder(ShortcutFolder obj, ShortcutFolder other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);

            if (obj.allFolders() != null)
            {
                if (other.allFolders() != null)
                {
                    foreach (ShortcutFolder subElement in obj.allFolders())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (ShortcutFolder otherElement in other.allFolders())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidShortcutFolder(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (ShortcutFolder otherElement in other.allFolders())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidShortcutFolder(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidShortcutFolder(subElement, null);
                        }
                    }

                    foreach (ShortcutFolder otherElement in other.allFolders())
                    {
                        bool found = false;
                        foreach (ShortcutFolder subElement in obj.allFolders())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidShortcutFolder(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (ShortcutFolder subElement in obj.allFolders())
                    {
                        ensureGuidShortcutFolder(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allFolders() != null)
                {
                    foreach (ShortcutFolder otherElement in other.allFolders())
                    {
                        ensureGuidShortcutFolder(null, otherElement);
                    }
                }
            }
            if (obj.allShortcuts() != null)
            {
                if (other.allShortcuts() != null)
                {
                    foreach (Shortcut subElement in obj.allShortcuts())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Shortcut otherElement in other.allShortcuts())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidShortcut(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Shortcut otherElement in other.allShortcuts())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidShortcut(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidShortcut(subElement, null);
                        }
                    }

                    foreach (Shortcut otherElement in other.allShortcuts())
                    {
                        bool found = false;
                        foreach (Shortcut subElement in obj.allShortcuts())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidShortcut(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Shortcut subElement in obj.allShortcuts())
                    {
                        ensureGuidShortcut(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allShortcuts() != null)
                {
                    foreach (Shortcut otherElement in other.allShortcuts())
                    {
                        ensureGuidShortcut(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two Shortcut have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidShortcut(Shortcut obj, Shortcut other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);
        }

        /// <summary>
        /// Ensures that two RequirementSet have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidRequirementSet(RequirementSet obj, RequirementSet other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);

            if (obj.allDependancies() != null)
            {
                if (other.allDependancies() != null)
                {
                    foreach (RequirementSetDependancy subElement in obj.allDependancies())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (RequirementSetDependancy otherElement in other.allDependancies())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidRequirementSetDependancy(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (RequirementSetDependancy otherElement in other.allDependancies())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidRequirementSetDependancy(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRequirementSetDependancy(subElement, null);
                        }
                    }

                    foreach (RequirementSetDependancy otherElement in other.allDependancies())
                    {
                        bool found = false;
                        foreach (RequirementSetDependancy subElement in obj.allDependancies())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRequirementSetDependancy(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (RequirementSetDependancy subElement in obj.allDependancies())
                    {
                        ensureGuidRequirementSetDependancy(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allDependancies() != null)
                {
                    foreach (RequirementSetDependancy otherElement in other.allDependancies())
                    {
                        ensureGuidRequirementSetDependancy(null, otherElement);
                    }
                }
            }
            if (obj.allSubSets() != null)
            {
                if (other.allSubSets() != null)
                {
                    foreach (RequirementSet subElement in obj.allSubSets())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (RequirementSet otherElement in other.allSubSets())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidRequirementSet(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (RequirementSet otherElement in other.allSubSets())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidRequirementSet(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRequirementSet(subElement, null);
                        }
                    }

                    foreach (RequirementSet otherElement in other.allSubSets())
                    {
                        bool found = false;
                        foreach (RequirementSet subElement in obj.allSubSets())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRequirementSet(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (RequirementSet subElement in obj.allSubSets())
                    {
                        ensureGuidRequirementSet(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allSubSets() != null)
                {
                    foreach (RequirementSet otherElement in other.allSubSets())
                    {
                        ensureGuidRequirementSet(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two RequirementSetDependancy have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidRequirementSetDependancy(RequirementSetDependancy obj, RequirementSetDependancy other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);
        }

        /// <summary>
        /// Ensures that two Specification have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidSpecification(Generated.Specification obj, Generated.Specification other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);

            if (obj.allChapters() != null)
            {
                if (other.allChapters() != null)
                {
                    foreach (Chapter subElement in obj.allChapters())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Chapter otherElement in other.allChapters())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.getId(), otherElement.getId()) && otherElement.getGuid() == null)
                            {
                                ensureGuidChapter(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Chapter otherElement in other.allChapters())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.getId(), otherElement.getId()))
                                {
                                    ensureGuidChapter(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidChapter(subElement, null);
                        }
                    }

                    foreach (Chapter otherElement in other.allChapters())
                    {
                        bool found = false;
                        foreach (Chapter subElement in obj.allChapters())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.getId(), otherElement.getId()))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidChapter(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Chapter subElement in obj.allChapters())
                    {
                        ensureGuidChapter(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allChapters() != null)
                {
                    foreach (Chapter otherElement in other.allChapters())
                    {
                        ensureGuidChapter(null, otherElement);
                    }
                }
            }
            if (obj.allChapterRefs() != null)
            {
                if (other.allChapterRefs() != null)
                {
                    foreach (ChapterRef subElement in obj.allChapterRefs())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (ChapterRef otherElement in other.allChapterRefs())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidChapterRef(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (ChapterRef otherElement in other.allChapterRefs())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidChapterRef(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidChapterRef(subElement, null);
                        }
                    }

                    foreach (ChapterRef otherElement in other.allChapterRefs())
                    {
                        bool found = false;
                        foreach (ChapterRef subElement in obj.allChapterRefs())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidChapterRef(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (ChapterRef subElement in obj.allChapterRefs())
                    {
                        ensureGuidChapterRef(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allChapterRefs() != null)
                {
                    foreach (ChapterRef otherElement in other.allChapterRefs())
                    {
                        ensureGuidChapterRef(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two ChapterRef have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidChapterRef(ChapterRef obj, ChapterRef other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);
        }

        /// <summary>
        /// Ensures that two Chapter have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidChapter(Chapter obj, Chapter other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidNamable(obj, other);

            if (obj.allParagraphs() != null)
            {
                if (other.allParagraphs() != null)
                {
                    foreach (Paragraph subElement in obj.allParagraphs())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Paragraph otherElement in other.allParagraphs())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.getId(), otherElement.getId()) && otherElement.getGuid() == null)
                            {
                                ensureGuidParagraph(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Paragraph otherElement in other.allParagraphs())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.getId(), otherElement.getId()))
                                {
                                    ensureGuidParagraph(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidParagraph(subElement, null);
                        }
                    }

                    foreach (Paragraph otherElement in other.allParagraphs())
                    {
                        bool found = false;
                        foreach (Paragraph subElement in obj.allParagraphs())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.getId(), otherElement.getId()))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidParagraph(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Paragraph subElement in obj.allParagraphs())
                    {
                        ensureGuidParagraph(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allParagraphs() != null)
                {
                    foreach (Paragraph otherElement in other.allParagraphs())
                    {
                        ensureGuidParagraph(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two Paragraph have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidParagraph(Paragraph obj, Paragraph other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }

            ensureGuidReferencesParagraph(obj, other);

            if (obj.allParagraphs() != null)
            {
                if (other.allParagraphs() != null)
                {
                    foreach (Paragraph subElement in obj.allParagraphs())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (Paragraph otherElement in other.allParagraphs())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.getId(), otherElement.getId()) && otherElement.getGuid() == null)
                            {
                                ensureGuidParagraph(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (Paragraph otherElement in other.allParagraphs())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.getId(), otherElement.getId()))
                                {
                                    ensureGuidParagraph(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidParagraph(subElement, null);
                        }
                    }

                    foreach (Paragraph otherElement in other.allParagraphs())
                    {
                        bool found = false;
                        foreach (Paragraph subElement in obj.allParagraphs())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.getId(), otherElement.getId()))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidParagraph(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (Paragraph subElement in obj.allParagraphs())
                    {
                        ensureGuidParagraph(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allParagraphs() != null)
                {
                    foreach (Paragraph otherElement in other.allParagraphs())
                    {
                        ensureGuidParagraph(null, otherElement);
                    }
                }
            }
            ensureGuidParagraphRevision(obj.getRevision(), other.getRevision());
            if (obj.allRequirementSets() != null)
            {
                if (other.allRequirementSets() != null)
                {
                    foreach (RequirementSetReference subElement in obj.allRequirementSets())
                    {
                        bool found = false;

                        // Try first to assign Guid to elements which do not have a guid
                        // This helps handling duplicated in lists
                        foreach (RequirementSetReference otherElement in other.allRequirementSets())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name) && otherElement.getGuid() == null)
                            {
                                ensureGuidRequirementSetReference(subElement, otherElement);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            foreach (RequirementSetReference otherElement in other.allRequirementSets())
                            {
                                if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                                {
                                    ensureGuidRequirementSetReference(subElement, otherElement);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRequirementSetReference(subElement, null);
                        }
                    }

                    foreach (RequirementSetReference otherElement in other.allRequirementSets())
                    {
                        bool found = false;
                        foreach (RequirementSetReference subElement in obj.allRequirementSets())
                        {
                            if (CompareUtil.canonicalStringEquality(subElement.Name, otherElement.Name))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ensureGuidRequirementSetReference(null, otherElement);
                        }
                    }
                }
                else
                {
                    foreach (RequirementSetReference subElement in obj.allRequirementSets())
                    {
                        ensureGuidRequirementSetReference(subElement, null);
                    }
                }
            }
            else
            {
                if (other.allRequirementSets() != null)
                {
                    foreach (RequirementSetReference otherElement in other.allRequirementSets())
                    {
                        ensureGuidRequirementSetReference(null, otherElement);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that two RequirementSetReference have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidRequirementSetReference(RequirementSetReference obj, RequirementSetReference other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }
        }

        /// <summary>
        /// Ensures that two ParagraphRevision have matching GUID, and recursively.
        /// obj is the leader for Guid. If other doesn't match obj guid, 
        ///   1. other does not have a guid, in that case, other should have the same guid as obj
        ///   2. other already has a guid. In that case, there is a mismatch between objects, and the process stops here
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void ensureGuidParagraphRevision(ParagraphRevision obj, ParagraphRevision other)
        {
            if (obj == null)
            {
                if (other != null)
                {
                    // Side effect, setup a GUID if needed for the other part (other)
                    string guid = other.Guid;
                }
                return;
            }

            if (other == null)
            {
                if (obj != null)
                {
                    // Side effect, setup a GUID if needed for the other part (obj)
                    string guid = obj.Guid;
                }
                return;
            }

            if (obj.Guid != other.getGuid())
            {
                if (string.IsNullOrEmpty(other.getGuid()))
                {
                    // These are matching elements, copy the guid from  obj
                    other.setGuid(obj.Guid);
                }
                else
                {
                    // Elements do not match. Stop the recursive process
                    return;
                }
            }
        }

        /// <summary>
        /// Searches a specific string in Namable and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchNamable(Generated.Namable obj, string searchString, List<ModelElement> occurences)
        {
            if (obj.getName() != null && obj.getName().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in ReferencesParagraph and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchReferencesParagraph(Generated.ReferencesParagraph obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.allRequirements() != null)
            {
                foreach (Generated.ReqRef subElement in obj.allRequirements())
                {
                    searchReqRef(subElement, searchString, occurences);
                }
            }
            if (obj.getComment() != null && obj.getComment().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in ReqRelated and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchReqRelated(Generated.ReqRelated obj, string searchString, List<ModelElement> occurences)
        {
            searchReferencesParagraph(obj, searchString, occurences);
        }

        /// <summary>
        /// Searches a specific string in Dictionary and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchDictionary(Generated.Dictionary obj, string searchString, List<ModelElement> occurences)
        {
            if (obj.allSpecifications() != null)
            {
                foreach (Generated.Specification subElement in obj.allSpecifications())
                {
                    searchSpecification(subElement, searchString, occurences);
                }
            }
            if (obj.allRequirementSets() != null)
            {
                foreach (RequirementSet subElement in obj.allRequirementSets())
                {
                    searchRequirementSet(subElement, searchString, occurences);
                }
            }
            if (obj.allRuleDisablings() != null)
            {
                foreach (RuleDisabling subElement in obj.allRuleDisablings())
                {
                    searchRuleDisabling(subElement, searchString, occurences);
                }
            }
            if (obj.allNameSpaces() != null)
            {
                foreach (NameSpace subElement in obj.allNameSpaces())
                {
                    searchNameSpace(subElement, searchString, occurences);
                }
            }
            if (obj.allNameSpaceRefs() != null)
            {
                foreach (NameSpaceRef subElement in obj.allNameSpaceRefs())
                {
                    searchNameSpaceRef(subElement, searchString, occurences);
                }
            }
            if (obj.allTests() != null)
            {
                foreach (Frame subElement in obj.allTests())
                {
                    searchFrame(subElement, searchString, occurences);
                }
            }
            if (obj.allTestRefs() != null)
            {
                foreach (FrameRef subElement in obj.allTestRefs())
                {
                    searchFrameRef(subElement, searchString, occurences);
                }
            }
            if (obj.getTranslationDictionary() != null)
            {
                searchTranslationDictionary(obj.getTranslationDictionary(), searchString, occurences);
            }
            if (obj.getShortcutDictionary() != null)
            {
                searchShortcutDictionary(obj.getShortcutDictionary(), searchString, occurences);
            }
            if (obj.getXsi() != null && obj.getXsi().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getXsiLocation() != null && obj.getXsiLocation().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in RuleDisabling and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchRuleDisabling(RuleDisabling obj, string searchString, List<ModelElement> occurences)
        {
            searchReqRelated(obj, searchString, occurences);

            if (obj.getRule() != null && obj.getRule().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in NameSpaceRef and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchNameSpaceRef(NameSpaceRef obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);
        }

        /// <summary>
        /// Searches a specific string in NameSpace and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchNameSpace(NameSpace obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.allNameSpaces() != null)
            {
                foreach (NameSpace subElement in obj.allNameSpaces())
                {
                    searchNameSpace(subElement, searchString, occurences);
                }
            }
            if (obj.allNameSpaceRefs() != null)
            {
                foreach (NameSpaceRef subElement in obj.allNameSpaceRefs())
                {
                    searchNameSpaceRef(subElement, searchString, occurences);
                }
            }
            if (obj.allRanges() != null)
            {
                foreach (Range subElement in obj.allRanges())
                {
                    searchRange(subElement, searchString, occurences);
                }
            }
            if (obj.allEnumerations() != null)
            {
                foreach (Enum subElement in obj.allEnumerations())
                {
                    searchEnum(subElement, searchString, occurences);
                }
            }
            if (obj.allStructures() != null)
            {
                foreach (Structure subElement in obj.allStructures())
                {
                    searchStructure(subElement, searchString, occurences);
                }
            }
            if (obj.allCollections() != null)
            {
                foreach (Collection subElement in obj.allCollections())
                {
                    searchCollection(subElement, searchString, occurences);
                }
            }
            if (obj.allStateMachines() != null)
            {
                foreach (StateMachine subElement in obj.allStateMachines())
                {
                    searchStateMachine(subElement, searchString, occurences);
                }
            }
            if (obj.allFunctions() != null)
            {
                foreach (Function subElement in obj.allFunctions())
                {
                    searchFunction(subElement, searchString, occurences);
                }
            }
            if (obj.allProcedures() != null)
            {
                foreach (Procedure subElement in obj.allProcedures())
                {
                    searchProcedure(subElement, searchString, occurences);
                }
            }
            if (obj.allVariables() != null)
            {
                foreach (Variable subElement in obj.allVariables())
                {
                    searchVariable(subElement, searchString, occurences);
                }
            }
            if (obj.allRules() != null)
            {
                foreach (Rule subElement in obj.allRules())
                {
                    searchRule(subElement, searchString, occurences);
                }
            }
            if (obj.getComment() != null && obj.getComment().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in ReqRef and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchReqRef(Generated.ReqRef obj, string searchString, List<ModelElement> occurences)
        {
            if (obj.getId() != null && obj.getId().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getSpecId() != null && obj.getSpecId().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getComment() != null && obj.getComment().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in Type and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchType(Type obj, string searchString, List<ModelElement> occurences)
        {
            searchReqRelated(obj, searchString, occurences);

            if (obj.getDefault() != null && obj.getDefault().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in Enum and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchEnum(Enum obj, string searchString, List<ModelElement> occurences)
        {
            searchType(obj, searchString, occurences);

            if (obj.allValues() != null)
            {
                foreach (EnumValue subElement in obj.allValues())
                {
                    searchEnumValue(subElement, searchString, occurences);
                }
            }
            if (obj.allSubEnums() != null)
            {
                foreach (Enum subElement in obj.allSubEnums())
                {
                    searchEnum(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in EnumValue and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchEnumValue(EnumValue obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.getValue() != null && obj.getValue().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in Range and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchRange(Range obj, string searchString, List<ModelElement> occurences)
        {
            searchType(obj, searchString, occurences);

            if (obj.getMinValue() != null && obj.getMinValue().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getMaxValue() != null && obj.getMaxValue().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.allSpecialValues() != null)
            {
                foreach (EnumValue subElement in obj.allSpecialValues())
                {
                    searchEnumValue(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in Structure and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchStructure(Structure obj, string searchString, List<ModelElement> occurences)
        {
            searchType(obj, searchString, occurences);

            if (obj.allElements() != null)
            {
                foreach (StructureElement subElement in obj.allElements())
                {
                    searchStructureElement(subElement, searchString, occurences);
                }
            }
            if (obj.allProcedures() != null)
            {
                foreach (Procedure subElement in obj.allProcedures())
                {
                    searchProcedure(subElement, searchString, occurences);
                }
            }
            if (obj.allStateMachines() != null)
            {
                foreach (StateMachine subElement in obj.allStateMachines())
                {
                    searchStateMachine(subElement, searchString, occurences);
                }
            }
            if (obj.allRules() != null)
            {
                foreach (Rule subElement in obj.allRules())
                {
                    searchRule(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in StructureElement and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchStructureElement(StructureElement obj, string searchString, List<ModelElement> occurences)
        {
            searchReqRelated(obj, searchString, occurences);

            if (obj.getTypeName() != null && obj.getTypeName().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getDefault() != null && obj.getDefault().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in Collection and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchCollection(Collection obj, string searchString, List<ModelElement> occurences)
        {
            searchType(obj, searchString, occurences);

            if (obj.getTypeName() != null && obj.getTypeName().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in Function and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchFunction(Function obj, string searchString, List<ModelElement> occurences)
        {
            searchType(obj, searchString, occurences);

            if (obj.allParameters() != null)
            {
                foreach (Generated.Parameter subElement in obj.allParameters())
                {
                    searchParameter(subElement, searchString, occurences);
                }
            }
            if (obj.allCases() != null)
            {
                foreach (Case subElement in obj.allCases())
                {
                    searchCase(subElement, searchString, occurences);
                }
            }
            if (obj.getTypeName() != null && obj.getTypeName().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in Parameter and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchParameter(Generated.Parameter obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.getTypeName() != null && obj.getTypeName().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in Case and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchCase(Case obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.allPreConditions() != null)
            {
                foreach (PreCondition subElement in obj.allPreConditions())
                {
                    searchPreCondition(subElement, searchString, occurences);
                }
            }
            if (obj.getExpression() != null && obj.getExpression().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getComment() != null && obj.getComment().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in Procedure and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchProcedure(Procedure obj, string searchString, List<ModelElement> occurences)
        {
            searchReqRelated(obj, searchString, occurences);

            if (obj.getStateMachine() != null)
            {
                searchStateMachine(obj.getStateMachine(), searchString, occurences);
            }
            if (obj.allRules() != null)
            {
                foreach (Rule subElement in obj.allRules())
                {
                    searchRule(subElement, searchString, occurences);
                }
            }
            if (obj.allParameters() != null)
            {
                foreach (Generated.Parameter subElement in obj.allParameters())
                {
                    searchParameter(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in StateMachine and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchStateMachine(StateMachine obj, string searchString, List<ModelElement> occurences)
        {
            searchType(obj, searchString, occurences);

            if (obj.getInitialState() != null && obj.getInitialState().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.allStates() != null)
            {
                foreach (State subElement in obj.allStates())
                {
                    searchState(subElement, searchString, occurences);
                }
            }
            if (obj.allRules() != null)
            {
                foreach (Rule subElement in obj.allRules())
                {
                    searchRule(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in State and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchState(State obj, string searchString, List<ModelElement> occurences)
        {
            searchReqRelated(obj, searchString, occurences);

            if (obj.getStateMachine() != null)
            {
                searchStateMachine(obj.getStateMachine(), searchString, occurences);
            }
            if (obj.getEnterAction() != null)
            {
                searchRule(obj.getEnterAction(), searchString, occurences);
            }
            if (obj.getLeaveAction() != null)
            {
                searchRule(obj.getLeaveAction(), searchString, occurences);
            }
        }

        /// <summary>
        /// Searches a specific string in Variable and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchVariable(Variable obj, string searchString, List<ModelElement> occurences)
        {
            searchReqRelated(obj, searchString, occurences);

            if (obj.getTypeName() != null && obj.getTypeName().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getDefaultValue() != null && obj.getDefaultValue().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.allSubVariables() != null)
            {
                foreach (Variable subElement in obj.allSubVariables())
                {
                    searchVariable(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in Rule and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchRule(Rule obj, string searchString, List<ModelElement> occurences)
        {
            searchReqRelated(obj, searchString, occurences);

            if (obj.allConditions() != null)
            {
                foreach (RuleCondition subElement in obj.allConditions())
                {
                    searchRuleCondition(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in RuleCondition and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchRuleCondition(RuleCondition obj, string searchString, List<ModelElement> occurences)
        {
            searchReqRelated(obj, searchString, occurences);

            if (obj.allPreConditions() != null)
            {
                foreach (PreCondition subElement in obj.allPreConditions())
                {
                    searchPreCondition(subElement, searchString, occurences);
                }
            }
            if (obj.allActions() != null)
            {
                foreach (Action subElement in obj.allActions())
                {
                    searchAction(subElement, searchString, occurences);
                }
            }
            if (obj.allSubRules() != null)
            {
                foreach (Rule subElement in obj.allSubRules())
                {
                    searchRule(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in PreCondition and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchPreCondition(PreCondition obj, string searchString, List<ModelElement> occurences)
        {
            if (obj.getCondition() != null && obj.getCondition().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getComment() != null && obj.getComment().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in Action and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchAction(Action obj, string searchString, List<ModelElement> occurences)
        {
            if (obj.getExpression() != null && obj.getExpression().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getComment() != null && obj.getComment().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in FrameRef and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchFrameRef(FrameRef obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);
        }

        /// <summary>
        /// Searches a specific string in Frame and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchFrame(Frame obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.getCycleDuration() != null && obj.getCycleDuration().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.allSubSequences() != null)
            {
                foreach (SubSequence subElement in obj.allSubSequences())
                {
                    searchSubSequence(subElement, searchString, occurences);
                }
            }
            if (obj.getComment() != null && obj.getComment().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in SubSequence and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchSubSequence(SubSequence obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.getD_LRBG() != null && obj.getD_LRBG().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getLevel() != null && obj.getLevel().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getMode() != null && obj.getMode().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getNID_LRBG() != null && obj.getNID_LRBG().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getQ_DIRLRBG() != null && obj.getQ_DIRLRBG().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getQ_DIRTRAIN() != null && obj.getQ_DIRTRAIN().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getQ_DLRBG() != null && obj.getQ_DLRBG().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getRBC_ID() != null && obj.getRBC_ID().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getRBCPhone() != null && obj.getRBCPhone().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.allTestCases() != null)
            {
                foreach (TestCase subElement in obj.allTestCases())
                {
                    searchTestCase(subElement, searchString, occurences);
                }
            }
            if (obj.getComment() != null && obj.getComment().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in TestCase and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchTestCase(TestCase obj, string searchString, List<ModelElement> occurences)
        {
            searchReqRelated(obj, searchString, occurences);

            if (obj.allSteps() != null)
            {
                foreach (Step subElement in obj.allSteps())
                {
                    searchStep(subElement, searchString, occurences);
                }
            }
            if (obj.getObsoleteComment() != null && obj.getObsoleteComment().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in Step and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchStep(Step obj, string searchString, List<ModelElement> occurences)
        {
            searchReferencesParagraph(obj, searchString, occurences);

            if (obj.getDescription() != null && obj.getDescription().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getObsoleteComment() != null && obj.getObsoleteComment().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getUserComment() != null && obj.getUserComment().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.allSubSteps() != null)
            {
                foreach (SubStep subElement in obj.allSubSteps())
                {
                    searchSubStep(subElement, searchString, occurences);
                }
            }
            if (obj.allMessages() != null)
            {
                foreach (DBMessage subElement in obj.allMessages())
                {
                    searchDBMessage(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in SubStep and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchSubStep(SubStep obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.allActions() != null)
            {
                foreach (Action subElement in obj.allActions())
                {
                    searchAction(subElement, searchString, occurences);
                }
            }
            if (obj.allExpectations() != null)
            {
                foreach (Expectation subElement in obj.allExpectations())
                {
                    searchExpectation(subElement, searchString, occurences);
                }
            }
            if (obj.getComment() != null && obj.getComment().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in Expectation and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchExpectation(Expectation obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.getValue() != null && obj.getValue().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getCondition() != null && obj.getCondition().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getComment() != null && obj.getComment().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in DBMessage and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchDBMessage(DBMessage obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.allFields() != null)
            {
                foreach (DBField subElement in obj.allFields())
                {
                    searchDBField(subElement, searchString, occurences);
                }
            }
            if (obj.allPackets() != null)
            {
                foreach (DBPacket subElement in obj.allPackets())
                {
                    searchDBPacket(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in DBPacket and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchDBPacket(DBPacket obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.allFields() != null)
            {
                foreach (DBField subElement in obj.allFields())
                {
                    searchDBField(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in DBField and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchDBField(DBField obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.getVariable() != null && obj.getVariable().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getValue() != null && obj.getValue().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in TranslationDictionary and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchTranslationDictionary(TranslationDictionary obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.allFolders() != null)
            {
                foreach (Folder subElement in obj.allFolders())
                {
                    searchFolder(subElement, searchString, occurences);
                }
            }
            if (obj.allTranslations() != null)
            {
                foreach (Translation subElement in obj.allTranslations())
                {
                    searchTranslation(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in Folder and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchFolder(Folder obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.allFolders() != null)
            {
                foreach (Folder subElement in obj.allFolders())
                {
                    searchFolder(subElement, searchString, occurences);
                }
            }
            if (obj.allTranslations() != null)
            {
                foreach (Translation subElement in obj.allTranslations())
                {
                    searchTranslation(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in Translation and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchTranslation(Translation obj, string searchString, List<ModelElement> occurences)
        {
            searchReferencesParagraph(obj, searchString, occurences);

            if (obj.allSourceTexts() != null)
            {
                foreach (SourceText subElement in obj.allSourceTexts())
                {
                    searchSourceText(subElement, searchString, occurences);
                }
            }
            if (obj.getObsoleteComment() != null && obj.getObsoleteComment().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.allSubSteps() != null)
            {
                foreach (SubStep subElement in obj.allSubSteps())
                {
                    searchSubStep(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in SourceText and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchSourceText(SourceText obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.allComments() != null)
            {
                foreach (SourceTextComment subElement in obj.allComments())
                {
                    searchSourceTextComment(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in SourceTextComment and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchSourceTextComment(SourceTextComment obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);
        }

        /// <summary>
        /// Searches a specific string in ShortcutDictionary and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchShortcutDictionary(ShortcutDictionary obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.allFolders() != null)
            {
                foreach (ShortcutFolder subElement in obj.allFolders())
                {
                    searchShortcutFolder(subElement, searchString, occurences);
                }
            }
            if (obj.allShortcuts() != null)
            {
                foreach (Shortcut subElement in obj.allShortcuts())
                {
                    searchShortcut(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in ShortcutFolder and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchShortcutFolder(ShortcutFolder obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.allFolders() != null)
            {
                foreach (ShortcutFolder subElement in obj.allFolders())
                {
                    searchShortcutFolder(subElement, searchString, occurences);
                }
            }
            if (obj.allShortcuts() != null)
            {
                foreach (Shortcut subElement in obj.allShortcuts())
                {
                    searchShortcut(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in Shortcut and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchShortcut(Shortcut obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.getShortcutName() != null && obj.getShortcutName().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in RequirementSet and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchRequirementSet(RequirementSet obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.allDependancies() != null)
            {
                foreach (RequirementSetDependancy subElement in obj.allDependancies())
                {
                    searchRequirementSetDependancy(subElement, searchString, occurences);
                }
            }
            if (obj.allSubSets() != null)
            {
                foreach (RequirementSet subElement in obj.allSubSets())
                {
                    searchRequirementSet(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in RequirementSetDependancy and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchRequirementSetDependancy(RequirementSetDependancy obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.getTarget() != null && obj.getTarget().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in Specification and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchSpecification(Generated.Specification obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.getVersion() != null && obj.getVersion().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.allChapters() != null)
            {
                foreach (Chapter subElement in obj.allChapters())
                {
                    searchChapter(subElement, searchString, occurences);
                }
            }
            if (obj.allChapterRefs() != null)
            {
                foreach (ChapterRef subElement in obj.allChapterRefs())
                {
                    searchChapterRef(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in ChapterRef and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchChapterRef(ChapterRef obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);
        }

        /// <summary>
        /// Searches a specific string in Chapter and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchChapter(Chapter obj, string searchString, List<ModelElement> occurences)
        {
            searchNamable(obj, searchString, occurences);

            if (obj.getId() != null && obj.getId().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.allParagraphs() != null)
            {
                foreach (Paragraph subElement in obj.allParagraphs())
                {
                    searchParagraph(subElement, searchString, occurences);
                }
            }
        }

        /// <summary>
        /// Searches a specific string in Paragraph and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchParagraph(Paragraph obj, string searchString, List<ModelElement> occurences)
        {
            searchReferencesParagraph(obj, searchString, occurences);

            if (obj.getId() != null && obj.getId().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getBl() != null && obj.getBl().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getText() != null && obj.getText().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getVersion() != null && obj.getVersion().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.allParagraphs() != null)
            {
                foreach (Paragraph subElement in obj.allParagraphs())
                {
                    searchParagraph(subElement, searchString, occurences);
                }
            }
            if (obj.getRevision() != null)
            {
                searchParagraphRevision(obj.getRevision(), searchString, occurences);
            }
            if (obj.getObsoleteFunctionalBlockName() != null && obj.getObsoleteFunctionalBlockName().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.allRequirementSets() != null)
            {
                foreach (RequirementSetReference subElement in obj.allRequirementSets())
                {
                    searchRequirementSetReference(subElement, searchString, occurences);
                }
            }
            if (obj.getObsoleteGuid() != null && obj.getObsoleteGuid().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in RequirementSetReference and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchRequirementSetReference(RequirementSetReference obj, string searchString, List<ModelElement> occurences)
        {
            if (obj.getTarget() != null && obj.getTarget().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }

        /// <summary>
        /// Searches a specific string in ParagraphRevision and updates the list 
        /// of model element with all the elements in which that string is found
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj">The string to search for</param>
        /// <param name="occurences">The list of model elements which hold the searched string</param>
        public static void searchParagraphRevision(ParagraphRevision obj, string searchString, List<ModelElement> occurences)
        {
            if (obj.getText() != null && obj.getText().Contains(searchString))
            {
                occurences.Add(obj);
            }
            if (obj.getVersion() != null && obj.getVersion().Contains(searchString))
            {
                occurences.Add(obj);
            }
        }
    }
}