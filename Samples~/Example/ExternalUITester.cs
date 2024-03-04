/*
  _    _                     _                                  _                 _   _         
 | |  | |                   | |                                | |               | | (_)        
 | |__| |   ___  __  __   __| |   ___     __ _   ______   ___  | |_   _   _    __| |  _    ___  
 |  __  |  / _ \ \ \/ /  / _` |  / _ \   / _` | |______| / __| | __| | | | |  / _` | | |  / _ \ 
 | |  | | |  __/  >  <  | (_| | | (_) | | (_| |          \__ \ | |_  | |_| | | (_| | | | | (_) |
 |_|  |_|  \___| /_/\_\  \__,_|  \___/   \__, |          |___/  \__|  \__,_|  \__,_| |_|  \___/ 
                                          __/ |                                                 
                                         |___/             
*/
using UnityEngine;
using HexTools.UI;

public class ExternalUITester : MonoBehaviour
{
  private int number;
  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.P))
    {
      var context = new Popup.Context(
        "Gift card #" + number++,
        "Congratulation you got a discount token. Heres your code:784V4A",
        "Accept",
        "Reject",
        (result) => Debug.Log("Result: " + result)
      );
      ExternalUIManager.Load("popup", context);
    }
  }
}
