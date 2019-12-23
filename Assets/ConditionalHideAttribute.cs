using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideAttribute : PropertyAttribute {
    public string controlField = "";
    public int showCondition = -1;
    public bool hideInInspector;

    public ConditionalHideAttribute(string controlField) {
        this.controlField = controlField;
        this.hideInInspector = false;
    }

    public ConditionalHideAttribute(string controlField, bool hideInInspector) {
        this.controlField = controlField;
        this.hideInInspector = hideInInspector;
    }

    public ConditionalHideAttribute(string controlField, int showCondition) {
        this.controlField = controlField;
        this.showCondition = showCondition;
        this.hideInInspector = false;
    }

    public ConditionalHideAttribute(string controlField, int showCondition, bool hideInInspector) {
        this.controlField = controlField;
        this.showCondition = showCondition;
        this.hideInInspector = hideInInspector;
    }
}
