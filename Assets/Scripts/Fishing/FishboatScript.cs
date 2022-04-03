using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishboatScript : MonoBehaviour
{
    public GameObject boat;
    public HookScript hook;
    public LineRenderer fishingLine;
    public bool isCastingLine;

    public Vector2 hookInitialPosition;
    public float maxCastDistance;
    public float castSpeed;
    public float movementSpeed;
    public float boatWobbleSpeed;
    public Vector3 rotationDirection;
    public ChoiceUIScript choiceUIScript;
    // Start is called before the first frame update
    void Start()
    {
        rotationDirection = Vector3.back;
    }

    // Update is called once per frame
    void Update()
    {
        if (choiceUIScript.gameObject.activeInHierarchy) {
            // no input recorded when ui is on
            return;
        }
        // only allow casting when not casting
        if (!isCastingLine && Input.GetKeyDown(KeyCode.DownArrow)) {
            StartCoroutine(CastLine());
        }
        if (!isCastingLine) {
            Vector2 potentialTranslation = Vector2.right * Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
            Vector2 potentialPosition = (Vector2) transform.position + potentialTranslation;
            if (potentialPosition.x < 8f && potentialPosition.x > -8f) {
                transform.Translate(potentialTranslation);
            }
        }
        if (boat.transform.rotation.z > 0.05) {
            rotationDirection = Vector3.back;
        } else if (boat.transform.rotation.z < -0.05) {
            rotationDirection = Vector3.forward;
        }
        boat.transform.Rotate(rotationDirection * Time.deltaTime * boatWobbleSpeed);
    }

    IEnumerator CastLine() {
        isCastingLine = true;
        hookInitialPosition = hook.transform.position;
        fishingLine.enabled = true;
        fishingLine.SetPositions(new Vector3[] {hookInitialPosition, hookInitialPosition});
        while (((Vector2) hook.transform.position - hookInitialPosition).magnitude < maxCastDistance && !hook.caughtFish) {
            hook.transform.Translate(Vector2.down * castSpeed * Time.deltaTime);
            fishingLine.SetPosition(1, hook.transform.position);
            yield return null;
        }
        while ((hookInitialPosition - (Vector2) hook.transform.position).magnitude > 0.025) {
            hook.transform.Translate((hookInitialPosition - (Vector2) hook.transform.position).normalized * castSpeed * Time.deltaTime);
            fishingLine.SetPosition(1, hook.transform.position);
            yield return null;
        }
        // TODO: process fish with UI?
        // Use it; scrap it; keep it for later
        if (hook.caughtFish) {
            RenderChoiceUI(hook.caughtFish);
        }
        isCastingLine = false;
        fishingLine.enabled = false;
    }

    void RenderChoiceUI(IdeaFishScript caughtFish) {
        choiceUIScript.AddFish(caughtFish);
        hook.caughtFish = null;
        Destroy(caughtFish.gameObject);
    }
}