using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public GameObject tutorial;

    private TextMeshProUGUI debug;

    void Start()
    {
        debug = GameObject.FindGameObjectWithTag("Debug").GetComponent<TextMeshProUGUI>();

        tutorial.transform.Find("TutorialClose").GetComponent<Button>().onClick.AddListener(() => Close());
    }

    // Update is called once per frame
    void Update()
    {
        var tutorialTitle = tutorial.transform.Find("TutorialTitle").GetComponent<TextMeshProUGUI>();
        var tutorialText = tutorial.transform.Find("TutorialText").GetComponent<TextMeshProUGUI>();

        int currentObjectIndex = CarAssemblyManager.currentObjectIndex;

        switch (currentObjectIndex)
        {
            case 0:
                tutorialTitle.text = "Clear Vision, Safe Journey";
                tutorialText.text = "The windows of a car and its wipers play an essential role in ensuring a safe and clear view for drivers, no matter the weather conditions. These two components work in harmony to provide visibility and maintain the comfort of passengers inside the vehicle.";
                break;
            case 1:
                tutorialTitle.text = "Beyond Functionality";
                tutorialText.text = "In the world of automobiles, certain components go beyond their basic functionality and become iconic features that captivate both drivers and enthusiasts. Among these notable elements are the frunk, door handles, and mirrors, each serving a unique purpose while adding a touch of style to the overall design of a car.";
                break;
            case 2:
                tutorialTitle.text = "Breathing Life into the Drive";
                tutorialText.text = "The air system of a car is an intricate network of components designed to ensure a comfortable and healthy driving experience. From the grille to the air filter and air vents, each element plays a crucial role in maintaining optimal air quality, temperature control, and overall cabin comfort.";
                break;
            case 3:
                tutorialTitle.text = "Illuminating the Road";
                tutorialText.text = "The lighting system of a car serves as both a functional necessity and a design element, enhancing visibility, safety, and overall aesthetics. From the headlights and lower headlights to the taillights and turn indicators, each component plays a crucial role in illuminating the path ahead and communicating the driver's intentions to others on the road.";
                break;
            case 4:
                tutorialTitle.text = "Rearward Enchantment";
                tutorialText.text = "The rear side of a car is a captivating space that combines functionality, performance, and visual appeal. It encompasses several noteworthy components such as the tailpipe, back bumper, and spoiler, each contributing to the overall design and performance of the vehicle.";
                break;
            case 5:
                tutorialTitle.text = "Where Rubber Meets the Road";
                tutorialText.text = "Tires are the unsung heroes of the automotive world, connecting the car to the road and playing a pivotal role in the overall performance, safety, and comfort of the vehicle. These round, rubber marvels are the primary point of contact with the ground, absorbing shocks, providing traction, and ensuring a smooth and controlled driving experience.";
                break;
            case 6:
                tutorialTitle.text = "Beyond the Badge";
                tutorialText.text = "The logo on a car is much more than a mere symbol. It is an emblem of identity, craftsmanship, and brand recognition. Positioned prominently on the vehicle's grille, trunk, or wheel caps, the logo serves as a visual representation of the car manufacturer's legacy, values, and reputation.";
                break;
        }
    }

    public void Close()
    {
        debug.text = "closing tutorial";
        tutorial.SetActive(false);
    }
}
