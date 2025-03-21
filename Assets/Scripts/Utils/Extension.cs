using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using Define;
using System;
using UnityEngine.EventSystems;

public static class Extension
{
	// Ȯ�� �޼���
	public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
	{
		return UIUtils.GetOrAddComponent<T>(go);
	}

	// Ȯ��޼���
	public static void BindEvent(this GameObject go, Action<PointerEventData> action, UIDefine.UIEvent type = UIDefine.UIEvent.Click)
	{
		BaseUI.BindEvent(go, action, type);
	}
}
