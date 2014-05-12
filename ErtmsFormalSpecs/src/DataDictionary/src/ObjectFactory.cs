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

namespace DataDictionary
{
    /// <summary>
    /// Sets the default values of the objects
    /// </summary>
    public class DefaultValueSetter : Generated.Visitor
    {
        /// <summary>
        /// Indicates that the Guid should be automatically set when creating a new object
        /// </summary>
        public bool AutomaticallyGenerateGuid { get; set; }

        public override void visit(Generated.BaseModelElement obj, bool visitSubNodes)
        {
            ModelElement element = (ModelElement)obj;

            if (AutomaticallyGenerateGuid)
            {
                // Side effect : creates the guid of the element
                string guid = element.Guid;
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.ReqRelated obj, bool visitSubNodes)
        {
            obj.setImplemented(false);
            obj.setVerified(false);
            obj.setNeedsRequirement(true);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.NameSpace obj, bool visitSubNodes)
        {
            obj.setX(0);
            obj.setY(0);
            obj.setWidth(0);
            obj.setHeight(0);
            obj.setHidden(false);
            obj.setPinned(false);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.EnumValue obj, bool visitSubNodes)
        {
            obj.setValue("0");

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Type obj, bool visitSubNodes)
        {
            obj.setX(0);
            obj.setY(0);
            obj.setWidth(0);
            obj.setHeight(0);
            obj.setHidden(false);
            obj.setPinned(false);
            obj.setDefault("");

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Range obj, bool visitSubNodes)
        {
            obj.setPrecision(Generated.acceptor.PrecisionEnum.aIntegerPrecision);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.StructureElement obj, bool visitSubNodes)
        {
            obj.setDefault("");
            obj.setMode(Generated.acceptor.VariableModeEnumType.aInternal);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Collection obj, bool visitSubNodes)
        {
            obj.setMaxSize(10);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Function obj, bool visitSubNodes)
        {
            obj.setCacheable(false);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Procedure obj, bool visitSubNodes)
        {
            obj.setX(0);
            obj.setY(0);
            obj.setWidth(0);
            obj.setHeight(0);
            obj.setHidden(false);
            obj.setPinned(false);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.State obj, bool visitSubNodes)
        {
            obj.setX(0);
            obj.setY(0);
            obj.setWidth(0);
            obj.setHeight(0);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Variable obj, bool visitSubNodes)
        {
            obj.setVariableMode(Generated.acceptor.VariableModeEnumType.aInternal);
            obj.setX(0);
            obj.setY(0);
            obj.setWidth(0);
            obj.setHeight(0);
            obj.setHidden(false);
            obj.setPinned(false);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Rule obj, bool visitSubNodes)
        {
            obj.setPriority(Generated.acceptor.RulePriority.aProcessing);
            obj.setX(0);
            obj.setY(0);
            obj.setWidth(0);
            obj.setHeight(0);
            obj.setHidden(false);
            obj.setPinned(false);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Frame obj, bool visitSubNodes)
        {
            obj.setCycleDuration("0.1");

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.SubSequence obj, bool visitSubNodes)
        {
            obj.setD_LRBG("");
            obj.setLevel("");
            obj.setMode("");
            obj.setNID_LRBG("");
            obj.setQ_DIRLRBG("");
            obj.setQ_DIRTRAIN("");
            obj.setQ_DLRBG("");
            obj.setRBC_ID("");
            obj.setRBCPhone("");

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.TestCase obj, bool visitSubNodes)
        {
            obj.setFeature(9999);
            obj.setCase(9999);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Step obj, bool visitSubNodes)
        {
            obj.setTCS_Order(0);
            obj.setDistance(0);
            obj.setIO(Generated.acceptor.ST_IO.StIO_NA);
            obj.setLevelIN(Generated.acceptor.ST_LEVEL.StLevel_NA);
            obj.setLevelOUT(Generated.acceptor.ST_LEVEL.StLevel_NA);
            obj.setModeIN(Generated.acceptor.ST_MODE.Mode_NA);
            obj.setModeOUT(Generated.acceptor.ST_MODE.Mode_NA);
            obj.setTranslationRequired(true);
            obj.setTranslated(false);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.SubStep obj, bool visitSubNodes)
        {
            obj.setSkipEngine(false);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Expectation obj, bool visitSubNodes)
        {
            obj.setKind(Generated.acceptor.ExpectationKind.aInstantaneous);
            obj.setBlocking(true);
            obj.setDeadLine(1);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.DBMessage obj, bool visitSubNodes)
        {
            obj.setMessageOrder(0);
            obj.setMessageType(Generated.acceptor.DBMessageType.aEUROBALISE);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.DBField obj, bool visitSubNodes)
        {
            obj.setValue(0);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Translation obj, bool visitSubNodes)
        {
            obj.setImplemented(false);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Paragraph obj, bool visitSubNodes)
        {
            obj.setGuid("");
            obj.setType(Generated.acceptor.Paragraph_type.aREQUIREMENT);
            obj.setObsoleteScope(Generated.acceptor.Paragraph_scope.aFLAGS);
            obj.setObsoleteScopeOnBoard(false);
            obj.setObsoleteScopeTrackside(false);
            obj.setObsoleteScopeRollingStock(false);
            obj.setBl("");
            obj.setOptional(true);
            obj.setName("");
            obj.setReviewed(false);
            obj.setImplementationStatus(Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NA);
            obj.setTested(false);
            obj.setVersion("3.0.0");
            obj.setMoreInfoRequired(false);
            obj.setSpecIssue(false);
            obj.setObsoleteFunctionalBlock(false);
            obj.setObsoleteFunctionalBlockName("");

            base.visit(obj, visitSubNodes);
        }
    }

    /// <summary>
    /// The factory
    /// </summary>
    public class ObjectFactory : Generated.Factory
    {
        /// <summary>
        /// The class used to set the default values
        /// </summary>
        private DefaultValueSetter DefaultValueSetter = new DefaultValueSetter();

        /// <summary>
        /// Indicates that the Guid should be automatically set when creating a new object
        /// </summary>
        public bool AutomaticallyGenerateGuid
        {
            get
            {
                return DefaultValueSetter.AutomaticallyGenerateGuid;
            }
            set
            {
                DefaultValueSetter.AutomaticallyGenerateGuid = value;
            }
        }

        public override Generated.Dictionary createDictionary()
        {
            Generated.Dictionary retVal = new Dictionary();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.ReqRef createReqRef()
        {
            Generated.ReqRef retVal = new ReqRef();

            DefaultValueSetter.visit(retVal);
            return retVal;

        }

        public override Generated.RuleDisabling createRuleDisabling()
        {
            Generated.RuleDisabling retVal = new Rules.RuleDisabling();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.NameSpace createNameSpace()
        {
            Generated.NameSpace retVal = new Types.NameSpace();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.NameSpaceRef createNameSpaceRef()
        {
            Generated.NameSpaceRef retVal = new Types.NameSpaceRef();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Enum createEnum()
        {
            Generated.Enum retVal = new Types.Enum();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.EnumValue createEnumValue()
        {
            Generated.EnumValue retVal = new Constants.EnumValue();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Range createRange()
        {
            Generated.Range retVal = new Types.Range();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Structure createStructure()
        {
            Generated.Structure retVal = new Types.Structure();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Collection createCollection()
        {
            Generated.Collection retVal = new Types.Collection();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.StructureElement createStructureElement()
        {
            Generated.StructureElement retVal = new Types.StructureElement();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Function createFunction()
        {
            Generated.Function retVal = new Functions.Function();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Parameter createParameter()
        {
            Generated.Parameter retVal = new Parameter();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Case createCase()
        {
            Generated.Case retVal = new Functions.Case();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Procedure createProcedure()
        {
            Generated.Procedure retVal = new Functions.Procedure();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.StateMachine createStateMachine()
        {
            Generated.StateMachine retVal = new Types.StateMachine();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.State createState()
        {
            Generated.State retVal = new Constants.State();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Variable createVariable()
        {
            Generated.Variable retVal = new Variables.Variable();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Rule createRule()
        {
            Generated.Rule retVal = new Rules.Rule();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.RuleCondition createRuleCondition()
        {
            Generated.RuleCondition retVal = new Rules.RuleCondition();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.PreCondition createPreCondition()
        {
            Generated.PreCondition retVal = new Rules.PreCondition();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Action createAction()
        {
            Generated.Action retVal = new Rules.Action();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.FrameRef createFrameRef()
        {
            Generated.FrameRef retVal = new Tests.FrameRef();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Frame createFrame()
        {
            Generated.Frame retVal = new Tests.Frame();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.SubSequence createSubSequence()
        {
            Generated.SubSequence retVal = new Tests.SubSequence();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.TestCase createTestCase()
        {
            Generated.TestCase retVal = new Tests.TestCase();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Step createStep()
        {
            Generated.Step retVal = new Tests.Step();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.SubStep createSubStep()
        {
            Generated.SubStep retVal = new Tests.SubStep();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Expectation createExpectation()
        {
            Generated.Expectation retVal = new Tests.Expectation();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.DBMessage createDBMessage()
        {
            Generated.DBMessage retVal = new Tests.DBElements.DBMessage();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.DBPacket createDBPacket()
        {
            Generated.DBPacket retVal = new Tests.DBElements.DBPacket();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.DBField createDBField()
        {
            Generated.DBField retVal = new Tests.DBElements.DBField();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Specification createSpecification()
        {
            Generated.Specification retVal = new Specification.Specification();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.RequirementSet createRequirementSet()
        {
            Generated.RequirementSet retVal = new Specification.RequirementSet();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.RequirementSetDependancy createRequirementSetDependancy()
        {
            Generated.RequirementSetDependancy retVal = new Specification.RequirementSetDependancy();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.RequirementSetReference createRequirementSetReference()
        {
            Generated.RequirementSetReference retVal = new Specification.RequirementSetReference();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Chapter createChapter()
        {
            Generated.Chapter retVal = new Specification.Chapter();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.ChapterRef createChapterRef()
        {
            Generated.ChapterRef retVal = new Specification.ChapterRef();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Paragraph createParagraph()
        {
            Generated.Paragraph retVal = new Specification.Paragraph();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Message createMessage()
        {
            Generated.Message retVal = new Specification.Message();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.MsgVariable createMsgVariable()
        {
            Generated.MsgVariable retVal = new Specification.MsgVariable();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.TypeSpec createTypeSpec()
        {
            Generated.TypeSpec retVal = new Specification.TypeSpec();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Values createValues()
        {
            Generated.Values retVal = new Specification.Values();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.special_or_reserved_values createspecial_or_reserved_values()
        {
            Generated.special_or_reserved_values retVal = new Specification.SpecialOrReservedValues();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.special_or_reserved_value createspecial_or_reserved_value()
        {
            Generated.special_or_reserved_value retVal = new Specification.SpecialOrReservedValue();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.mask createmask()
        {
            Generated.mask retVal = new Specification.Mask();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.match creatematch()
        {
            Generated.match retVal = new Specification.Match();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.meaning createmeaning()
        {
            Generated.meaning retVal = new Specification.Meaning();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.match_range creatematch_range()
        {
            Generated.match_range retVal = new Specification.MatchRange();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.resolution_formula createresolution_formula()
        {
            Generated.resolution_formula retVal = new Specification.ResolutionFormula();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.value createvalue()
        {
            Generated.value retVal = new Specification.Value();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.char_value createchar_value()
        {
            Generated.char_value retVal = new Specification.CharValue();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.ParagraphRevision createParagraphRevision()
        {
            Generated.ParagraphRevision retVal = new Specification.ParagraphRevision();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.TranslationDictionary createTranslationDictionary()
        {
            Generated.TranslationDictionary retVal = new Tests.Translations.TranslationDictionary();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Folder createFolder()
        {
            Generated.Folder retVal = new Tests.Translations.Folder();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Translation createTranslation()
        {
            Generated.Translation retVal = new Tests.Translations.Translation();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.SourceText createSourceText()
        {
            Generated.SourceText retVal = new Tests.Translations.SourceText();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.ShortcutDictionary createShortcutDictionary()
        {
            Generated.ShortcutDictionary retVal = new Shortcuts.ShortcutDictionary();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.ShortcutFolder createShortcutFolder()
        {
            Generated.ShortcutFolder retVal = new Shortcuts.ShortcutFolder();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }

        public override Generated.Shortcut createShortcut()
        {
            Generated.Shortcut retVal = new Shortcuts.Shortcut();

            DefaultValueSetter.visit(retVal);

            return retVal;
        }
    }
}
