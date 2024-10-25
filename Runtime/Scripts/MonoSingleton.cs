/// <summary>
/// Script:  MonoSingleton.cs
/// Author:  AaronBuffie
/// </summary>
using UnityEngine;
namespace Aarware.Core{
abstract public class MonoSingleton<T> : MonoBehaviour where T : Object {

	/// <summary>
	/// The static instance
	/// </summary>
	/// <remarks>
	/// NOTE: Take care accessing any 'i' from another Mono's Awake, be sure this object existsed at least the frame before or you might get null back
	/// Best practice is to not access other monos in the Awake()
	/// </remarks>
	/// <value></value>
	public static T i {get; private set;}

	/// <summary>
	/// Must be called from private void Awake()
	/// </summary>
	/// <remarks>
	/// dontDestroy     - Should persist between scenes;
	/// killGameObject  - Destroy whole gameObject or just the componenet on exists
	/// </remarks>      
	/// <param name="dontDestroy"></param>
	/// <param name="killGameObject"></param>
	protected void Awake(bool dontDestroy = false, bool killGameObject=true) {
		if (i == null) {
			i = this as T;
		} else if (i != this) {
			Destroy((killGameObject ? gameObject : this));
			return;
		}

		if (dontDestroy) {
			DontDestroyOnLoad(gameObject);
		}
	}
}
}    