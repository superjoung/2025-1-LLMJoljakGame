using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Util
{
    public class UIUtils
    {
        public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : Object
        {
            if (go == null)
                return null;

            // 직계 자식들만 검수
            if (recursive == false)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Transform transform = go.transform.GetChild(i);
                    if (string.IsNullOrEmpty(name) || transform.name == name)
                    {
                        T component = transform.GetComponent<T>();
                        if (component != null)
                            return component;
                    }
                }
            }
            // 직계 && 증손자 모든 자식들 검수
            else
            {
                foreach (T component in go.GetComponentsInChildren<T>())
                {
                    if (string.IsNullOrEmpty(name) || component.name == name)
                        return component;
                }
            }
            return null;
        }

        // 컴포넌트 없는 빈 깡통일 때만 사용
        public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
        {
            Transform transform = FindChild<Transform>(go, name, recursive);
            if (transform == null)
                return null;

            return transform.gameObject;
        }

        public static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            if (component == null)
                component = go.AddComponent<T>();
            return component;
        }
    }

    // 화면 터치 확인
    public static class TouchUtils
    {
        public enum TouchState { None, Began, Moved, Ended }; //모바일과 에디터 모두 가능하게 터치 & 마우스 처리
                                                                   // 모바일 or 에디터 마우스 터치좌표 처리
        public static void TouchSetUp(ref TouchState touchState, ref Vector2 touchPosition)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject() == false) { touchState = TouchState.Began; }
            }
            else if (Input.GetMouseButton(0)) { if (EventSystem.current.IsPointerOverGameObject() == false) { touchState = TouchState.Moved; } }
            else if (Input.GetMouseButtonUp(0)) { if (EventSystem.current.IsPointerOverGameObject() == false) { touchState = TouchState.Ended; } }
            else touchState = TouchState.None;
            touchPosition = Input.mousePosition;
#else
                if (Input.touchCount > 0)
                {

                    Touch touch = Input.GetTouch(0);
                    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId) == true) return;
                    if (touch.phase == TouchPhase.Began) touchState = ETouchState.Began;
                    else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) touchState = ETouchState.Moved;
                    else if (touch.phase == TouchPhase.Ended) if (touchState != ETouchState.None) touchState = ETouchState.Ended;
                    touchPosition = touch.position;
                }
                else touchState = ETouchState.None;
#endif
        }
    }
}

namespace Define
{
    public class UIDefine
    {
        public enum UIEvent
        {
            Click,
            Drag,
        }

        public enum MouseEvent
        {
            Press,
            Click,
        }
    }
}
