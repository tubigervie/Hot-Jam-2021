using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    // Using Collider over raycast due to the small size of the pickup colliders
    [SerializeField] Collider interactionCollider;
    [SerializeField] AudioClip pickupSFX;

    public IPickable pickup = null;
    public IInteractable interactable = null;

    public Ingredient currentIngredient { get { return _currentIngredient; } }
    public CauldronContainer cauldronContainer { get { return _cauldronSlot; } }

    [SerializeField] Ingredient _currentIngredient = null;
    [SerializeField] CauldronContainer _cauldronSlot = null;


    [SerializeField] HelpText helpText;
    [SerializeField] DebugPanelController debugPanel;

    AudioSource audioSource;

    private CharacterController charController;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        charController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        helpText = GameObject.Find("PlayerCanvas").GetComponent<HelpText>();
        debugPanel = GameObject.Find("DebugPanel").GetComponent<DebugPanelController>();
        debugPanel.TogglePanel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (pickup != null)     // If current focus is IPickable
            {
                if (_currentIngredient != null) {
                    DropIngredient();
                }
                else if(_cauldronSlot != null)
                {
                    cauldronContainer.GetComponent<CauldronAI>().Drop(this.transform);
                    SetCauldronSlot(null);
                }
                audioSource.PlayOneShot(pickupSFX);
                SetCurrentItem(pickup.PickUp());
                pickup = null;
            }
            else if (interactable != null)  // If current focus is IInteractable
            {
                interactable.Interact(this.gameObject);
            }
            else if (_currentIngredient != null)
            {
                DropIngredient();
            }
            else if (_cauldronSlot != null)
            {
                cauldronContainer.GetComponent<CauldronAI>().Drop(this.transform);
                SetCauldronSlot(null);
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            debugPanel.TogglePanel();
        }

        // Debug Panel Updates
        if (_currentIngredient != null)
        {
            debugPanel.UpdateHeldObj(_currentIngredient.displayName);
        }
        else if (_cauldronSlot != null)
        {
            debugPanel.UpdateHeldObj("Cauldron");
        }
        else
        {
            debugPanel.UpdateHeldObj("N/A");
        }
        if (pickup != null)
        {
            debugPanel.UpdateDetectedObj(pickup.ToString());
        }
        else if (interactable != null)
        {
            debugPanel.UpdateDetectedObj(interactable.ToString());
        }
        else
        {
            debugPanel.UpdateDetectedObj("N/A");
        }
    }

    private void FixedUpdate()
    {
        InterfaceCheck();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, .75f);
    }

    private void InterfaceCheck()
    {
        float closestObjectDistance = float.PositiveInfinity;
        Collider closestPickableCollider = null;
        Collider[] triggerColliders = Physics.OverlapSphere(transform.position + transform.forward, .75f);
        foreach (Collider collider in triggerColliders)
        {
            IPickable item = collider.gameObject.GetComponent<IPickable>();
            IInteractable tool = collider.gameObject.GetComponent<IInteractable>();
            if (item != null || tool != null)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestObjectDistance)
                {
                    closestObjectDistance = distance;
                    closestPickableCollider = collider;
                }
            }
        }
        if(closestPickableCollider == null)
        {
            pickup = null;
            interactable = null;
            helpText.ToggleInteractIcon(false, null);
            if(_currentIngredient != null)
            {
                helpText.TogglePickableIcon(true, this.gameObject);
            }
            else
            {
                helpText.TogglePickableIcon(false, null);
            }
            return;
        }

        IPickable closestPickable = closestPickableCollider.GetComponent<IPickable>(); //just to get null
        IInteractable closestInteractable = closestPickableCollider.GetComponent<IInteractable>(); //just to get null

        if (closestPickable != null)   // If collided obj has IPickable
        {
            pickup = closestPickable;
            helpText.TogglePickableIcon(true, closestPickableCollider.gameObject);
            helpText.ToggleInteractIcon(true, this.transform.gameObject);
            interactable = null;
        }
        else if (closestInteractable != null) // If collided obj has IInteractable
        {
            interactable = closestInteractable;
            helpText.ToggleInteractIcon(true, closestPickableCollider.gameObject);
            pickup = null;
            if(currentIngredient == null)
                helpText.TogglePickableIcon(false, null);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    IPickable item = other.gameObject.GetComponent<IPickable>();
    //    IInteractable tool = other.gameObject.GetComponent<IInteractable>();
    //    if (item != null)   // If collided obj has IPickable
    //    {
    //        pickup = item;
    //        helpText.ToggleInteractIcon(true, other.gameObject);
    //        interactable = null;
    //    }
    //    else if (tool != null) // If collided obj has IInteractable
    //    {
    //        interactable = tool;
    //        helpText.ToggleInteractIcon(true, other.gameObject);
    //        pickup = null;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    IPickable item = other.gameObject.GetComponent<IPickable>();
    //    IInteractable tool = other.gameObject.GetComponent<IInteractable>();
    //    if (item != null)   // If collided obj has IPickable
    //    {
    //        pickup = null;
    //    }
    //    else if (tool != null)  // If collided obj has IInteractable
    //    {
    //        interactable = null;
    //    }
    //}

    public void SetCurrentItem(Ingredient item)
    {
        _currentIngredient = item;
    }

    public void SetCauldronSlot(CauldronContainer cauldron)
    {
        _cauldronSlot = cauldron;
    }

    public void DropIngredient()
    {
        _currentIngredient.SpawnPickup(transform.position + transform.forward);
        _currentIngredient = null;
    }
}
