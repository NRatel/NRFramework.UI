public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ constraint implement type
	// }} 

	// {{ AOT generic type
	//System.Action`1<System.Object>
	//System.Action`1<System.Net.Sockets.SocketError>
	//System.Action`1<System.Int32>
	//System.Action`2<System.Object,System.Byte>
	//System.Action`2<System.Object,System.Int32>
	//System.Collections.Generic.Dictionary`2<System.Object,System.Object>
	//System.Collections.Generic.Dictionary`2/Enumerator<System.Object,System.Object>
	//System.Collections.Generic.Dictionary`2/ValueCollection<System.Object,System.Object>
	//System.Collections.Generic.Dictionary`2/ValueCollection/Enumerator<System.Object,System.Object>
	//System.Collections.Generic.IEnumerator`1<System.Object>
	//System.Collections.Generic.KeyValuePair`2<System.Object,System.Object>
	//System.Collections.Generic.List`1<System.Object>
	//System.Collections.Generic.List`1/Enumerator<System.Object>
	//System.Comparison`1<System.Object>
	//System.Func`2<System.Object,System.Byte>
	//System.Predicate`1<System.Object>
	//UnityEngine.Events.UnityAction`1<System.Int32>
	//UnityEngine.Events.UnityAction`1<System.Single>
	//UnityEngine.Events.UnityAction`1<UnityEngine.Vector2>
	//UnityEngine.Events.UnityAction`1<System.Object>
	//UnityEngine.Events.UnityAction`1<System.Byte>
	//UnityEngine.Events.UnityEvent`1<System.Int32>
	//UnityEngine.Events.UnityEvent`1<System.Single>
	//UnityEngine.Events.UnityEvent`1<UnityEngine.Vector2>
	//UnityEngine.Events.UnityEvent`1<System.Object>
	//UnityEngine.Events.UnityEvent`1<System.Byte>
	// }}

	public void RefMethods()
	{
		// System.Object System.Array::Find<System.Object>(System.Object[],System.Predicate`1<System.Object>)
		// System.Object System.Threading.Interlocked::CompareExchange<System.Object>(System.Object&,System.Object,System.Object)
		// System.Object UnityEngine.Component::GetComponent<System.Object>()
		// System.Object UnityEngine.Component::GetComponentInParent<System.Object>()
		// System.Boolean UnityEngine.Component::TryGetComponent<System.Object>(System.Object&)
		// System.Object UnityEngine.GameObject::AddComponent<System.Object>()
		// System.Object UnityEngine.GameObject::GetComponent<System.Object>()
		// System.Object UnityEngine.Object::Instantiate<System.Object>(System.Object)
	}
}