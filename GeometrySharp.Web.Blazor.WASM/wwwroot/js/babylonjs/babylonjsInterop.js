var canvas;
var engine;

function createTestScene(engine, canvas) {
    var scene = new BABYLON.Scene(engine);
    scene.clearColor = BABYLON.Color3(0.7,0.7,0.7);
    // This creates and positions a free camera (non-mesh)
       const camera = new BABYLON.ArcRotateCamera("Camera", -Math.PI / 2, Math.PI / 4, 15, BABYLON.Vector3.Zero());
  
    // This attaches the camera to the canvas
    camera.attachControl(canvas, true);

    //Defines a light in the scene
    const light = new BABYLON.HemisphericLight("light", new BABYLON.Vector3(1, 1, 0));

    var groundMaterial = new BABYLON.GridMaterial("groundMaterial", scene);
    groundMaterial.majorUnitFrequency = 5;
    groundMaterial.minorUnitVisibility = 0.45;
    groundMaterial.gridRatio = 2;
    groundMaterial.backFaceCulling = false;
    groundMaterial.mainColor = new BABYLON.Color3(1, 1, 1);
    groundMaterial.lineColor = new BABYLON.Color3(0, 0, 0);
    groundMaterial.opacity = 0.98;

    //abstract plane from its position and normal
    const abstractPlane = BABYLON.Plane.FromPositionAndNormal(new BABYLON.Vector3(0, 0, 0), new BABYLON.Vector3(0, 1, 0));
    const ground = BABYLON.MeshBuilder.CreatePlane("plane", { sourcePlane: abstractPlane, sideOrientation: BABYLON.Mesh.DOUBLESIDE, size: 20 });

    ground.material = groundMaterial;

    return scene;
};

function InitializeBabylonScene(canvasId) {
    canvas = document.getElementById(canvasId); // Get the canvas element
    engine = new BABYLON.Engine(canvas, true); // Generate the BABYLON 3D engine

    const scene = createTestScene(engine, canvas); //Call the createTestScene function

    // Register a render loop to repeatedly render the scene
    engine.runRenderLoop(function () {
        scene.render();
    });

    // Watch for browser/canvas resize events
    window.addEventListener("resize", function () {
        engine.resize();
    });
}

function Line() {
    const myPoints = [
        new BABYLON.Vector3(-2, -1, 0),
        new BABYLON.Vector3(0, 1, 0),
        new BABYLON.Vector3(2, -1, 0),
    ]

    myPoints.push(myPoints[0]);

    const options = {
        points: myPoints,
        updatable: true,   
    }

    let lines = BABYLON.MeshBuilder.CreateLines("lines", options);
    lines.color = new BABYLON.Color4(0, 0, 0, 1);

}