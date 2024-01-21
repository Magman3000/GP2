using UnityEngine;
using UnityEngine.SceneManagement;


namespace Initialization {
    public class Initializer {

        [RuntimeInitializeOnLoadMethod]
        public static void InitializeGame() {

            var resource = Resources.Load<GameObject>("GameInstance");
            GameObject game = Object.Instantiate(resource);
            Object.DontDestroyOnLoad(game);

            GameInstance gameInstance = game.GetComponent<GameInstance>();
            gameInstance.Initialize();

           //ceneManager.GetSceneAt(0).GetRootGameObjects()[0].
        }
    }
}
