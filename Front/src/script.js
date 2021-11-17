import "./style.css";
import * as THREE from "three";
import { OrbitControls } from "three/examples/jsm/controls/OrbitControls.js";
import { PATH } from "./CONSTANTS";

const form = document.querySelector("form");
const xInput = document.querySelector("#xInput");
const yInput = document.querySelector("#yInput");
const zInput = document.querySelector("#zInput");
const boundary = document.querySelector("#boundary-select");
const neighborhoodType = document.querySelector("#neighborhood-select");
const nucleons = document.querySelector("#nucleons");
const numberOfIterations = document.querySelector("#numberOfIterations");
const kt = document.querySelector("#kt");
const monteCarloIterations = document.querySelector("#monteCarloIterations");
const button = document.querySelector(".generate__button");

const downloadToFile = (content, filename) => {
  const a = document.createElement("a");
  const file = new Blob([content], { type: "text/plain" });

  a.href = URL.createObjectURL(file);
  a.download = filename;
  a.click();

  URL.revokeObjectURL(a.href);
};

form.addEventListener("submit", (e) => {
  e.preventDefault();
  downloadToFile(
    [
      `\n\n\n\n\n\n\n\n\n\n\n\n${xInput.value}\n${yInput.value}\n${zInput.value}\n${boundary.value}\n${neighborhoodType.value}\n${nucleons.value}\n${numberOfIterations.value}\n${kt.value}\n${monteCarloIterations.value}`,
    ],
    "parameters1.txt"
  );
  RunCSharpServerExecutable();
});

function RunCSharpServerExecutable() {
  document.dispatchEvent(
    new CustomEvent("funcIntraLaunch", {
      detail: {
        task: "run",
        program: "MetodyDyskretne.exe",
        workingfolder: PATH,
        switches: "",
        log: "C:\\Windows\\run_log.txt",
        windowstate: "max",
        recallapp: "",
        options: "",
        playsound: "http://www.yourdomain.com/sound.wav",
        showerrors: "true",
      },
    })
  );
}

function main() {
  const canvas = document.querySelector("canvas.webgl");
  const renderer = new THREE.WebGLRenderer({ canvas });

  const camera = new THREE.PerspectiveCamera(75, 2, 0.1, 200);
  camera.position.z = 40;
  camera.position.x = 40;
  camera.position.y = 40;

  const controls = new OrbitControls(camera, canvas);
  controls.target.set(0, 0, 0);
  controls.update();

  const scene = new THREE.Scene();
  scene.background = new THREE.Color("black");

  function addLight(...pos) {
    const color = 0xffffff;
    const intensity = 2;
    const light = new THREE.DirectionalLight(color, intensity);
    light.position.set(...pos);
    scene.add(light);
  }
  addLight(-1, 2, 4);
  addLight(1, -1, -2);
  addLight(-4, -2, -4);

  const boxWidth = 1;
  const boxHeight = 1;
  const boxDepth = 1;
  const geometry = new THREE.BoxGeometry(boxWidth, boxHeight, boxDepth);

  function makeInstance(geometry, color, x, y, z) {
    const material = new THREE.MeshPhongMaterial({
      color,
      opacity: 0.1,
      transparent: true,
    });

    const cube = new THREE.Mesh(geometry, material);
    scene.add(cube);
    cube.position.set(x, y, z);
    return cube;
  }

  function hsl(h, s, l) {
    return new THREE.Color().setHSL(h, s, l);
  }

  function optimizeIdArray(microstructureTxt) {
    let IdArray = [];
    for (let index = 0; index < microstructureTxt.length; index++) {
      if (
        microstructureTxt[index] != " " &&
        microstructureTxt[index + 1] == " "
      )
        IdArray.push(microstructureTxt[index]);
      if (
        microstructureTxt[index] == "\n" &&
        microstructureTxt[index + 2] !== "\n" &&
        microstructureTxt[index - 2] !== "\n"
      ) {
        IdArray.push("changeY");
      }
      if (
        microstructureTxt[index] == "\n" &&
        microstructureTxt[index + 2] == "\n"
      )
        IdArray.push("changeZ");
    }
    return IdArray;
  }

  function selectInputFile() {
    let file = document.querySelector("#file-input").files[0];
    let reader = new FileReader();
    reader.addEventListener("load", function (e) {
      let microstructureTxt = e.target.result;
      let IdArray = optimizeIdArray(microstructureTxt);
      drawCubes(IdArray);
    });
    reader.readAsText(file);
  }

  button.addEventListener("click", () => selectInputFile());

  function drawCubes(IdArray) {
    const d = 0.5;
    let x = 1;
    let y = 1;
    let z = 1;
    for (const iterator of IdArray) {
      if (/[0-9]/.test(parseInt(iterator))) {
        makeInstance(
          geometry,
          hsl(parseInt(iterator) / 8, 1, 0.5),
          x * d,
          y * d,
          z * d
        );
        x = x + 2;
      }
      if (iterator === "changeY") {
        x = 1;
        y = y + 2;
      }
      if (iterator === "changeZ") {
        x = 1;
        y = 1;
        z = z + 2;
      }
    }
  }

  function render() {
    renderRequested = undefined;

    if (resizeRendererToDisplaySize(renderer)) {
      const canvas = renderer.domElement;
      camera.aspect = canvas.clientWidth / canvas.clientHeight;
      camera.updateProjectionMatrix();
    }

    requestRenderIfNotRequested();
    renderer.render(scene, camera);
  }

  function resizeRendererToDisplaySize(renderer) {
    const canvas = renderer.domElement;
    const width = canvas.clientWidth;
    const height = canvas.clientHeight;
    const needResize = canvas.width !== width || canvas.height !== height;
    if (needResize) {
      renderer.setSize(width, height, false);
    }
    return needResize;
  }

  let renderRequested = false;

  render();

  function requestRenderIfNotRequested() {
    if (!renderRequested) {
      renderRequested = true;
      requestAnimationFrame(render);
    }
  }

  controls.addEventListener("change", requestRenderIfNotRequested);
  window.addEventListener("resize", requestRenderIfNotRequested);
}

main();
