(function () {
  const GRID_COLOR = "#ddd";
  const HIGHLIGHT_COLOR = "#99ccff";

  let _collisionOverlays;
  let _deleteOverlay;
  let _mapMenuOption;
  let _currentCollisionType;

  let _map;
  let _stage;

  let _visualLayer;
  let _collisionLayer;
  let _objectLayer;

  let _placementNode;
  let _highlightPlacementNode;

  function getCanvasDimensions() {
    const tilemapContainerBounds = document.getElementById("tilemapContainer").getBoundingClientRect();
    const width = tilemapContainerBounds.width;
    const height = document.getElementById("main").getBoundingClientRect().height - document.getElementById("menu").getBoundingClientRect().bottom - 14;

    return {
      width: width,
      height: height,
    };
  }
  function resizeUI() {
    let characterList = document.getElementById("characterlist");
    if (!characterList) {
      return;
    }

    const height = document.getElementById("main").getBoundingClientRect().height - characterList.getBoundingClientRect().top;
    characterList.style.height = `${height}px`;
  }
  async function createVisualLayerAsync() {
    if (!_stage || !_map) {
      throw new Error("Must call initialize.");
    }

    let visualLayer = new Konva.Layer();

    // Render background image.
    let imageGroup = new Konva.Group({
      id: "imageGroup",
    });
    visualLayer.add(imageGroup);

    let visualImage = await common.createImageAsync(_map.imageUrl);

    // Resize image to conform to tiles.
    const width = Math.ceil((visualImage.width / _map.tileWidth)) * _map.tileWidth;
    const height = Math.ceil((visualImage.height / _map.tileHeight)) * _map.tileHeight;

    if (width <= 0 || height <= 0) {
      throw new Error("Invalid image.");
    }

    let node = new Konva.Image({
      x: 0,
      y: 0,
      image: visualImage,
      width: width,
      height: height,
      id: "visual",
    });

    // Set maps dimensions.
    _map.width = Math.ceil(width / _map.tileWidth);
    _map.height = Math.ceil(height / _map.tileHeight);

    // Reset scrolling bounds with new dimensions.
    _stage.dragBoundFunc(position => {
      const offsetWidth = _stage.width() - (width * (_stage.scaleX()));
      const offsetHeight = _stage.height() - (height * (_stage.scaleY()));

      return {
        x: common.clamp(position.x, offsetWidth, 0),
        y: common.clamp(position.y, offsetHeight, 0),
      };
    });

    // Reset visual layer.
    imageGroup.add(node);

    // Render grid.
    let gridGroup = new Konva.Group({
      id: "grid",
    });
    visualLayer.add(gridGroup);

    // Create grid.
    for (let x = 0; x < _map.width; x++) {
      gridGroup.add(
        new Konva.Line({
          points: [Math.round(x * _map.tileWidth) + 0.5, 0, Math.round(x * _map.tileWidth) + 0.5, height],
          stroke: GRID_COLOR,
          strokeWidth: 1,
          listening: false,
        }));
    }

    gridGroup.add(
      new Konva.Line({
        points: [0, 0, 10, 10],
        listening: false,
      }));

    for (var y = 0; y < _map.height; y++) {
      gridGroup.add(
        new Konva.Line({
          points: [0, Math.round(y * _map.tileHeight), width, Math.round(y * _map.tileHeight)],
          stroke: GRID_COLOR,
          strokeWidth: 0.5,
          listening: false,
        }));
    }

    return visualLayer;
  }
  function createCollisionLayer() {
    if (!_stage || !_map) {
      throw new Error("Must call initialize.");
    }

    const width = _map.width;
    const height = _map.height;
    if (width <= 0 || height <= 0) {
      throw new Error("Map must have dimensions.");
    }

    let collisionLayer = new Konva.Layer({
      visible: false,
      opacity: 0.5,
    });
 
    _map.collisionTiles.forEach(t => {
        let node = new Konva.Image({
          x: Math.ceil(t.x * _map.tileWidth),
          y: Math.ceil(t.y * _map.tileHeight),
          width: _map.tileWidth,
          height: _map.tileHeight,
          image: t.collisionType === 0 ? null : _collisionOverlays[t.collisionType - 1],
        });

      collisionLayer.add(node);
    });

    return collisionLayer;
  }
  function createEmptyObjectLayer() {
    if (!_stage || !_map) {
      throw new Error("Must call initialize.");
    }

    let objectLayer = new Konva.Layer();
    let characterGroup = new Konva.Group({
      id: "characters",
      visible: false,
    });
    objectLayer.add(characterGroup);
    let gameObjectGroup = new Konva.Group({
      id: "gameObjects",
      visible: false,
    });
    objectLayer.add(gameObjectGroup);
    objectLayer.on("click", async e => {
      if (e.evt.button !== 0) {
        return;
      }

      // If there is a placement node create a new character.
      if (_placementNode) {
        const mouseCoordinates = getMouseCoordinates();

        let character = JSON.parse(_placementNode.name());
        character.x = common.clamp(mouseCoordinates.x, 0, _map.width - 1);
        character.y = common.clamp(mouseCoordinates.y, 0, _map.height - 1);
        character.id = await common.createGuidAsync(); 

        await addCharacterAsync(character);

        _objectLayer.batchDraw();
      }
    });

    return objectLayer;
  }

  async function createCharacterNodeAsync(character) {
    if (!character.imageUrl) {
      character.imageUrl = "/assets/question-mark.png";
    }
    let characterImage = await common.createImageAsync(character.imageUrl);
    let characterNode = new Konva.Image({
      image: characterImage,
    });

    characterNode.x(character.x * _map.tileWidth);
    characterNode.y(character.y * _map.tileHeight);
    characterNode.width(character.width * _map.tileWidth);
    characterNode.height(character.height * _map.tileHeight);
    characterNode.draggable(true);
    characterNode.id(character.id);
    characterNode.name(JSON.stringify(character));

    return characterNode;
  }
  async function addCharacterAsync(character) {
    if (!_stage || !_map) {
      throw new Error("Must call initialize.");
    }

    let characterNode = await createCharacterNodeAsync(character);
    let characterHighlightNode = new Konva.Rect({
      x: characterNode.x(),
      y: characterNode.y(),
      fill: HIGHLIGHT_COLOR,
      opacity: 0.6,
      stoke: HIGHLIGHT_COLOR,
      stokeWidth: 3,
      visible: false,
      width: characterNode.width(),
      height: characterNode.height(),
    });
    let group = _objectLayer.getChildren(c => c.id() === "characters")[0];
    group.add(characterNode);
    group.add(characterHighlightNode);

    _map.mapCharacters.push({
      id: character.id,
      characterId: character.characterId,
      x: character.x,
      y: character.y,
    });

    let transformerNode = new Konva.Transformer({
      node: characterNode,
      resizeEnabled: false,
      rotateEnabled: false,
      borderEnabled: false,
      visible: false,
    });
    group.add(transformerNode);

    const deleteNodeWidth = 16;
    let deleteNode = new Konva.Image({
      image: _deleteOverlay,
      x: transformerNode.width() - (deleteNodeWidth / 2),
      y: -(deleteNodeWidth / 2),
      width: deleteNodeWidth,
      height: deleteNodeWidth,
    });
    transformerNode.add(deleteNode);

    transformerNode.on("transform", e => {
      deleteNode.x(transformerNode.width());
    });

    deleteNode.on("click", e => {
      const id = characterNode.id();
      deleteNode.destroy();
      transformerNode.destroy();
      characterNode.destroy();

      let mapCharacter = _map.mapCharacters.find(c => c.id == id);
      let index = _map.mapCharacters.indexOf(mapCharacter);
      _map.mapCharacters.splice(index, 1);

      _objectLayer.batchDraw();
    });
    deleteNode.on("mouseover", e => {
      transformerNode.moveToTop();
      transformerNode.show();
      common.setCursorToPointer();
      _objectLayer.batchDraw();
    });
    deleteNode.on("mouseout", e => {
      transformerNode.hide();
      common.setCursorToDefault();
      _objectLayer.batchDraw();
    });

    characterNode.on("mouseover", e => {
      transformerNode.moveToTop();
      transformerNode.show();
      common.setCursorToPointer();
      _objectLayer.batchDraw();
    });
    characterNode.on("mouseout", e => {
      transformerNode.hide();
      common.setCursorToDefault();
      _objectLayer.batchDraw();
    });
    characterNode.on("dragstart", e => {
      characterHighlightNode.moveToTop();
      characterNode.moveToTop();
      transformerNode.moveToTop();
      characterHighlightNode.show();
    });
    characterNode.on("dragend", e => {
      characterHighlightNode.hide();

      const mouseCoordinates = getMouseCoordinates();
      const dx = common.clamp(mouseCoordinates.x, 0, _map.width - 1);
      const dy = common.clamp(mouseCoordinates.y, 0, _map.height - 1);

      let mapCharacter = _map.mapCharacters.find(c => c.id == characterNode.id());
      mapCharacter.x = dx;
      mapCharacter.y = dy;

      characterNode.x(dx * _map.tileWidth);
      characterNode.y(dy * _map.tileHeight);

      transformerNode.moveToTop();
      _objectLayer.batchDraw();
    });
    characterNode.on("dragmove", e => {
      const mouseCoordinates = getMouseCoordinates();
      const dx = common.clamp(mouseCoordinates.x, 0, _map.width - 1);
      const dy = common.clamp(mouseCoordinates.y, 0, _map.height - 1);

      characterHighlightNode.x(dx * _map.tileWidth);
      characterHighlightNode.y(dy * _map.tileHeight);
    });
  }
  function getMousePosition() {
    const scale = _stage.scaleX();
    let pointer = _stage.getPointerPosition();

    // When mouse is not on the canvas set position to zero.
    if (!pointer) {
      pointer = {
        x: 0,
        y: 0,
      };
    }

    return {
      x: (pointer.x - _stage.x()) / scale,
      y: (pointer.y - _stage.y()) / scale,
    };
  }
  function getMouseCoordinates() {
    const mousePosition = getMousePosition();


    return {
      x: Math.floor(mousePosition.x / _map.tileWidth),
      y: Math.floor(mousePosition.y / _map.tileHeight),
    };
  }
  function setCollisionOnNode(coordinates) {
    if (coordinates.x >= _map.width || coordinates.x < 0 || coordinates.y >= _map.height || coordinates.y < 0)
      return;

    let existingNode = _collisionLayer.getChildren(c =>
      c.x() === coordinates.x * _map.tileWidth &&
      c.y() === coordinates.y * _map.tileHeight)[0];

    if (existingNode) {
      existingNode.destroy();
      let tileIndex = _map.collisionTiles.indexOf(_map.collisionTiles.find(c => c.x === coordinates.x && c.y === coordinates.y));
      if (tileIndex > -1) {
        _map.collisionTiles.splice(tileIndex, 1);
      }
    }

    if (_currentCollisionType !== 0) {

      let node = new Konva.Image({
        x: Math.ceil(coordinates.x * _map.tileWidth),
        y: Math.ceil(coordinates.y * _map.tileHeight),
        width: _map.tileWidth,
        height: _map.tileHeight,
        image: _collisionOverlays[_currentCollisionType - 1],
      });

      _collisionLayer.add(node);

      _map.collisionTiles.push({
        collisionType: _currentCollisionType,
        x: coordinates.x,
        y: coordinates.y,
      });

    }
    _collisionLayer.batchDraw();
  }

  window.map = {
    initialize: async function (map) {
      _map = map;
      _mapMenuOption = 0;
      _currentCollisionType = 15;
      // Clear array for manual entry.
      _map.mapCharacters = [];

      // Load assets.
      const numberOfCollisionImages = 15;
      _collisionOverlays = new Array(numberOfCollisionImages);
      for (let i = 0; i < numberOfCollisionImages; i++) {
        _collisionOverlays[i] = await common.createImageAsync(`/assets/collision-tile-overlay-${i + 1}.png`);
      }

      //_deleteOverlay = await common.createImageAsync("/assets/delete-icon.png");
      _deleteOverlay = await common.createImageAsync("/assets/close-circle-red-512.png");

      const canvasDimensions = getCanvasDimensions();

      // Load main container.
      _stage = new Konva.Stage({
        container: "tilemapContainer",
        width: canvasDimensions.width,
        height: canvasDimensions.height,
        draggable: true,
      });
      _stage.on("contextmenu", e => {
        e.evt.preventDefault();
      });
      _stage.on("mousedown", e => {
        _stage.draggable(e.evt.button === 2);
      });
      window.addEventListener("resize", e => {
        window.requestAnimationFrame(() => {
          const canvasDimensions = getCanvasDimensions();

          _stage.width(canvasDimensions.width);
          _stage.height(canvasDimensions.height);

          resizeUI();
        });
      });

      // Load visual layer.
      _visualLayer = await createVisualLayerAsync();
      _stage.add(_visualLayer);

      // Load collision layer.
      _collisionLayer = createCollisionLayer();
      _stage.add(_collisionLayer);

      // Create an empty object layer, load objects individually.
      _objectLayer = createEmptyObjectLayer();
      _highlightPlacementNode = new Konva.Rect({
        fill: HIGHLIGHT_COLOR,
        opacity: 0.6,
        stoke: HIGHLIGHT_COLOR,
        stokeWidth: 3,
        visible: false,
      });
      _objectLayer.add(_highlightPlacementNode);
      _stage.add(_objectLayer);
      // Handle moving highlight/placement nodes.
      _stage.on("mousemove", e => {
        if (_mapMenuOption === 2 && _stage.getPointerPosition() && e.evt.buttons === 1) {
          setCollisionOnNode(getMouseCoordinates());
        }

        if (_mapMenuOption !== 0 && _mapMenuOption !== 1) {
          return;
        }
        if (!_stage.getPointerPosition()) {
          return;
        }

        let mousePosition = getMousePosition();
        let mouseCoordinates = getMouseCoordinates();
        mouseCoordinates = {
          x: common.clamp(mouseCoordinates.x, 0, _map.width - 1),
          y: common.clamp(mouseCoordinates.y, 0, _map.height - 1),
        };

        // Render placement node.
        if (_placementNode) {

          _highlightPlacementNode.x(mouseCoordinates.x * _map.tileWidth);
          _highlightPlacementNode.y(mouseCoordinates.y * _map.tileHeight);
          _highlightPlacementNode.moveToTop();

          _placementNode.x(mousePosition.x - (_placementNode.width() / 2));
          _placementNode.y(mousePosition.y - (_placementNode.height() / 2));
          _placementNode.moveToTop();

          _objectLayer.batchDraw();
        }

      });

      _stage.on("mousedown", e => {
          if (_mapMenuOption !== 2 || !_stage.getPointerPosition() || e.evt.button !== 0) {
            return;
          }

          setCollisionOnNode(getMouseCoordinates());
      });
    
      // Render changes.
      _stage.batchDraw();
    },
    toggleGrid: function (isVisible) {
      let gridGroup = _visualLayer.getChildren(c => c.id() === "grid")[0];
      gridGroup.visible(isVisible);
      _visualLayer.batchDraw();
    },
    setZoom: function (increment) {
      // Manually set value here since semantic UI creates a new div.
      document.getElementsByClassName("zoom")[0].innerHTML = `${increment}%`;

      let pointer = _stage.getPointerPosition();
      if (!pointer) {
        pointer = {
          x: 0,
          y: 0,
        };
      }

      const mousePointTo = getMousePosition();
      const newScale = increment / 100;

      _stage.scale({
        x: newScale,
        y: newScale,
      });

      const newPosition = {
        x: pointer.x - mousePointTo.x * newScale,
        y: pointer.y - mousePointTo.y * newScale,
      };

      const offsetWidth = _stage.width() - ((_map.width * _map.tileWidth) * _stage.scaleX());
      const offsetHeight = _stage.height() - ((_map.height * _map.tileHeight) * _stage.scaleY());

      _stage.position({
        x: common.clamp(newPosition.x, offsetWidth, 0),
        y: common.clamp(newPosition.y, offsetHeight, 0),
      });
      _stage.batchDraw();
    },
    setCollisionType: function (collisionType) {
      _currentCollisionType = collisionType;
    },
    setMapMenuOption: function (option) {
      _mapMenuOption = option;
      _objectLayer.hide();
      _collisionLayer.hide();

      var characterGroup = _objectLayer.getChildren(c => c.id() === "characters")[0];
      var gameObjectGroup = _objectLayer.getChildren(c => c.id() === "gameObjects")[0];

      characterGroup.hide();
      gameObjectGroup.hide();

      // Show characters.
      if (option === 0) {
        _objectLayer.show();
        characterGroup.show();
        _objectLayer.batchDraw();
      }
      // Show game objects.
      else if (option === 1) {
        _objectLayer.show();
        gameObjectGroup.show();
        _objectLayer.batchDraw();
      }
      // Show collision.
      else if (option === 2) {
        _collisionLayer.show();
        _collisionLayer.batchDraw();
      }
    },
    getMap: function () {
      return {
        width: _map.width,
        height: _map.height,
        collisionTiles: _map.collisionTiles,
        mapCharacters: _map.mapCharacters,
      };
    },
    addCharacter: async function (character) {
      await addCharacterAsync(character);
      _objectLayer.batchDraw();
    },
    setSelectedCharacter: async function (character) {
      if (_placementNode) {
        _placementNode.destroy();
        _placementNode = null;
        _highlightPlacementNode.hide();

        _objectLayer.batchDraw();
      }

      if (!character) {
        return;
      }

      _placementNode = await createCharacterNodeAsync(character);
      _highlightPlacementNode.width(_placementNode.width());
      _highlightPlacementNode.height(_placementNode.height());
      _highlightPlacementNode.show();

      _objectLayer.add(_placementNode);
    },
    resizeCharacterList: function () {
      window.requestAnimationFrame(() => {
        resizeUI();
      });
    },
  };

})();