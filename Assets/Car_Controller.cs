using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Volplane;
using Volplane.AirConsole;

[RequireComponent(typeof(Collider2D))]
public class Car_Controller : VolplaneBehaviour {

    // Public variables
    public int playerNumber = 0;        // Player id from which the input should be taken
    public int player0 = 0;
    public int player1 = 1;
    float vertical = 0f;
    float horizontal = 0f;
    [SerializeField] float horizontalAdd;
    [SerializeField] float horizontalSlow;
    [SerializeField] float speedForce;            // Speed of the car
    [SerializeField] float torqueForce;
    float driftFactorSticky = 0.9f;
    float driftFactorSlippy = 1;
    float maxStickyVelocity = 2.5f;
    float minSlippyVelocity = 1.5f;	// <--- Exercise for the viewer
    Rigidbody2D rb;

    private GameController_Cars main;

    /// <summary>
    /// 'MonoBehaviour.Start()' method from Unity
    /// Start is called on the frame when a script is enabled just before any of the Update methods
    /// is called the first time.
    /// </summary>
    private void Start() {
        // Reference to the Game Controller
        rb = GetComponent<Rigidbody2D>();
        main = GameObject.FindWithTag("GameController").GetComponent<GameController_Cars>();

    }
    /// <summary>
    /// 'MonoBehaviour.Update()' method from Unity
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update() {
        rb.velocity = ForwardVelocity() + RightVelocity();

        float driftFactor = ApplyDriftFactor();

        rb.velocity = ForwardVelocity() + RightVelocity() * driftFactor;

        VerticalCalc();
        
        HorizontalCalc();

        SetAngularVelocitiy();
    }

    Vector2 ForwardVelocity() {
        VolplaneController.AirConsole.SetOrientation("landscape");
        return transform.up * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.up);
    }

    Vector2 RightVelocity() {
        return transform.right * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.right);
    }

    private void VerticalCalc() {
        // Get button input from controller (name 'buttonUp')
        if (VInput.GetButton(player0, "buttonUp")) {
            print("Player: " + player0 + " pressed Up");
            // Move car up
            rb.AddForce(transform.up * speedForce);
        }
        // Get button input from controller (name 'buttonDown')
        else if (VInput.GetButton(player0, "buttonDown")) {
            print("Player: " + player0 + " pressed Down");
            // Move car down
            if (rb.velocity.y > -4f)
                rb.AddForce(transform.up * -speedForce);
        }
        else if (vertical > 0) {
            vertical += speedForce;
        }
        else if (vertical < 0) {
            vertical = Mathf.Clamp(vertical + rb.drag, vertical, 0);
        }
    }

    private void HorizontalCalc() {
        // Get button input from controller (name 'buttonLeft')
        if (VInput.GetButton(player1, "buttonLeft")) {
            print("Player: " + player1 + " pressed Left");
            // Turn car left
            horizontal += -horizontalAdd;
            horizontal = Mathf.Clamp(horizontal - horizontalAdd, -1f, 0);
        }
        // Get button input from controller (name 'buttonRight')
        else if (VInput.GetButton(player1, "buttonRight")) {
            print("Player: " + player1 + " pressed Right");
            // Turn car right
            horizontal = Mathf.Clamp(horizontal + horizontalAdd, 0, 1f);
        }
        else if (horizontal > 0) {
            horizontal = Mathf.Clamp(horizontal - horizontalAdd, -1f, 0);
        }
        else if (horizontal < 0) {
            horizontal = Mathf.Clamp(horizontal + horizontalSlow, horizontal, 0);
        }
    }

    private void SetAngularVelocitiy() {
        float tf = Mathf.Lerp(0, torqueForce, rb.velocity.magnitude / 2);
        rb.angularVelocity = horizontal * tf;
    }

    private float ApplyDriftFactor() {
        float driftFactor = driftFactorSticky;
        if (RightVelocity().magnitude > maxStickyVelocity) {
            driftFactor = driftFactorSlippy;
        }

        return driftFactor;
    }
}