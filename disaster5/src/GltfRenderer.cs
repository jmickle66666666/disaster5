// TODO: this

using glTFLoader;
using glTFLoader.Schema;

namespace Disaster {
    public class GltfRenderer : Renderer {
        Gltf model;
        public GltfRenderer(string path) {
            model = Interface.LoadModel(path);

            

            // model.Scenes[0].
            // model.Meshes[0].Primitives[0].
            // model.Meshes[0].Primitives[0].Indices
        }

        public void Render() {

        }

        public void Dispose() {

        }
    }
}