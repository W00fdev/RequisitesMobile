using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Assets.Scripts.Core;
using System.Text;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Assets.Scripts.Shared
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public abstract class SearchbarBase : MonoBehaviour
    {
        [SerializeField] int _layerOrder;
        public int LayerOrder 
        { 
            get => _layerOrder; 
            set
            {
                _layerOrder = Mathf.Clamp(value, 0, 100);
                NeedToRefreshLayer = true;
            }
        }
        public bool Initialized { get; set; } = false;

        protected UnityEvent<string> OnInputComplete = new UnityEvent<string>();
        protected ParserResponse DropdownResponse;
        protected bool NeedToRefreshLayer = true;
        protected bool empty = true;

        protected Coroutine Refresher = null;

        public virtual void Initialize(UnityAction<string> inputCompleteAction)
        {
            OnInputComplete.AddListener(inputCompleteAction);
            Initialized = true;

            SwitchState(true);
        }

        public void StartSearchbar()
        {
            InitializeOptionsFirst();
        }

        public void CloseSearchbar()
        {
            Dropdown.options.Clear();
            ClearOptions();
            InputField.text = "";
            SwitchState(false);
        }

        public void SwitchState(bool enabled)
        {
            if (enabled == false)
            {
                PreviousInput = "NOT INITIALIZED";
                //PreviousInputDropdown = "NOT INITIALIZED";
                Label.enabled = false;
                HintText.Text = "";

                DropdownResponse = new ParserResponse(ParserResponse.ResponseType.EMPTY);
                UpdateHint();
            }

            if (empty == true && enabled == true)
                InitializeOptionsFirst();

            SwitchStateInputField(enabled);
            SwitchStateDropdown(enabled);
        }

        protected void BaseAwake()
        {
            // <InputField>
            if (InputField == null)
            {
                foreach (Transform child in transform)
                {
                    if (child.TryGetComponent(out TMP_InputField inputFieldComponent))
                    {
                        InputField = inputFieldComponent;
                        break;
                    }
                }
            }

            InputField.characterValidation = TMP_InputField.CharacterValidation.Digit;
            InputField.characterLimit = CharacterLimit;
            InputField.onFocusSelectAll = false;
            InputField.onSelect.AddListener(SelectEventInputField);
            InputField.onValueChanged.AddListener(ChangeValueEventInputField);

            if (HintText == null)
            {
                bool hintFound = false;
                foreach (Transform child in InputField.transform)
                {
                    if (child.childCount > 0)
                    {
                        foreach (Transform grandChild in child)
                        {
                            if (grandChild.TryGetComponent(out HintText hintComponent))
                            {
                                HintText = hintComponent;
                                hintFound = true;
                                break;
                            }
                        }
                    }

                    if (hintFound)
                        break;
                }
            }
            // </InputField>



            // <Dropdown>
            Dropdown = Dropdown != null ? Dropdown : GetComponent<TMP_Dropdown>();
            Dropdown.onValueChanged.AddListener(ChooseValueEventDropdown);

            if (Label == null)
            {
                foreach (Transform child in transform)
                {
                    if (child.TryGetComponent(out TMP_Text labelComponent))
                    {
                        Label = labelComponent;
                        break;
                    }
                }
            }

            OptionString = new StringBuilder(50, 100);
            // </Dropdown>

            SwitchState(false);
        }

        // TODO: Remove from ShowDropdown().
        protected void UpdateLayerOrder()
        {
            if (NeedToRefreshLayer == false)
                return;

            InputField.GetComponent<Canvas>().sortingOrder = 30000 + (LayerOrder * 2) + 1;
            
            foreach(Transform child in transform)
            {
                if (child.GetComponent<TMP_InputField>() == null && child.TryGetComponent(out Canvas canvas))
                {
                    canvas.sortingOrder = 30000 + LayerOrder * 2;
                    NeedToRefreshLayer = false;
                    Debug.Log("Dropdown list has refreshed..." + canvas);
                }
            }

            //if (NeedToRefreshLayer == true && Refresher == null)
               // Refresher = StartCoroutine(RefreshLayerCheck());
        }

        // TODO: Delete this.
/*
        protected IEnumerator RefreshLayerCheck()
        {
            int i = 0;
            // 100 frames check
            while (i < 100 && NeedToRefreshLayer)
            {
                yield return null;
                UpdateLayerOrder();
                i++;
                //Debug.Log(i);
            }
        }*/

        // --------------------------- <DROPDOWN> ---------------------------
        [Header("Dropdown: ")]
        [SerializeField] protected TMP_Dropdown Dropdown;
        [SerializeField] protected TMP_Text Label;

        // Cached options and variables.
        protected List<TMP_Dropdown.OptionData>[] OptionsCached;
        protected StringBuilder OptionString;
        protected string PreviousInputDropdown;

        // In Dropdown elements are started from '1', because of 0's index is filled by string.Empty.
        protected readonly TMP_Dropdown.OptionData EmptyOption = new TMP_Dropdown.OptionData("");
        protected const int FIRST = 1;

        protected virtual void SwitchStateDropdown(bool enabled)
        {
            Dropdown.interactable = enabled;

            if (enabled == false)
                HideDropdown();
        }

        protected virtual void ChooseValueEventDropdown(int optionIndex)
        {
            string optionString = Dropdown.options[optionIndex].text;
            DropdownResponse = new ParserResponse(ParserResponse.ResponseType.OK, optionString);
            UpdateInput(optionString);
        }

        protected virtual bool CheckUpdatingDropdown(string newInput)
        {
            if (Initialized == false)
            {
                DropdownResponse = new ParserResponse(ParserResponse.ResponseType.EMPTY);
                return true;
            }

            if (PreviousInputDropdown == newInput)
            {
                return true;
            }

            return false;
        }

        protected virtual void ShowDropdown()
        {
            Dropdown.RefreshShownValue();
            Dropdown.Show();
            UpdateLayerOrder();
        }

        protected virtual void HideDropdown()
        {
            Dropdown.Hide();
        }

        protected abstract ParserResponse UpdateDropdown(string newInput);
        protected abstract void InitializeOptionsFirst();
        protected abstract void SelectEventInputField(string newInput);
        protected abstract void ClearOptions();


        // --------------------------- </DROPDOWN> ---------------------------


        // --------------------------- <INPUTFIELD> ---------------------------
        [Header("InputField: ")]
        [SerializeField] protected TMP_InputField InputField;
        [SerializeField] protected HintText HintText;
        [SerializeField] protected int CharacterLimit;

        protected string PreviousInput;

        protected virtual string ParseValue(string source)
        {
            if (source.IndexOf(' ') == -1)
                return "";

            string parsedValue = source.Substring(0, source.IndexOf(' '));

            if (parsedValue.Length > CharacterLimit)
                Debug.LogError($"Option has more than {CharacterLimit} digits...");

            return parsedValue;
        }

        protected virtual void SwitchStateInputField(bool enabled)
        {
            InputField.interactable = enabled;

            if (enabled == true)
                InputField.ActivateInputField();
            else
                InputField.DeactivateInputField();
        }

        protected void SelectInputField()
        {
            InputField.Select();

            InputField.selectionAnchorPosition = 0;
            InputField.selectionFocusPosition = InputField.text.Length;
        }

        protected abstract void UpdateHint();
        protected abstract void UpdateInput(string newInput);
        protected abstract void ChangeValueEventInputField(string newInput);


        // --------------------------- </INPUTFIELD> ---------------------------
    }
}

