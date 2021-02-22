using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testbutton : MonoBehaviour
{
   public GameObject AztecSelected;
   public GameObject TutorialSelected;
   public GameObject TidesSelected;

   public void SelectAztec()
   {
      TutorialSelected.SetActive(false);
      TidesSelected.SetActive(false);
      AztecSelected.SetActive(true);
   }
   public void SelectTides()
   {
      TutorialSelected.SetActive(false);
      AztecSelected.SetActive(false);
      TidesSelected.SetActive(true);
   }
   public void SelectTutorial()
   {
      AztecSelected.SetActive(false);
      TidesSelected.SetActive(false);
      TutorialSelected.SetActive(true);
   }
}
