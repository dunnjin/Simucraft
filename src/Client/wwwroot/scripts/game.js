(function () {
  const GRID_COLOR = "#ddd";
  const HIGHLIGHT_COLOR = "#99ccff";
  const MOVE_COLOR = "#0000ff";
  const TARGET_COLOR = "#ff0000";

  let _game;
  let _gameState;
  let _clientState;
  let _stage;

  let _visualLayer;
  let _objectLayer;
  let _interactiveLayer;
  let _fogOfWarLayer;

  let _placementNode;
  let _highlightPlacementNode;
  let _selectedHightlightNode;
  let _selectedAnimation;
  let _previousSelectedGameCharacterId;

  let _deleteOverlay;
  let _isGridVisible;

  let _deletedGameCharacterId;
  let _draggingGameCharacter;

  function getCanvasDimensions() {
    const gameContainerBounds = document.getElementById("gameContainer").getBoundingClientRect();
    const width = gameContainerBounds.width;
    const height = document.getElementById("main").getBoundingClientRect().height - document.getElementById("menu").getBoundingClientRect().bottom;

    return {
      width: width,
      height: height,
    };
  }

  function resizeUI() {
    let gameruleset = document.getElementById("gameruleset");
    if (gameruleset) {
      const height = document.getElementById("main").getBoundingClientRect().height - gameruleset.getBoundingClientRect().top;
      gameruleset.style.height = `${height}px`;
    }
    let gamecharacterinfo = document.getElementById("gamecharacterinfo");
    if (gamecharacterinfo) {
      const height = document.getElementById("main").getBoundingClientRect().height - gamecharacterinfo.getBoundingClientRect().top;
      gamecharacterinfo.style.height = `${height}px`;
    }
    let gamecharacterbook = document.getElementById("gamecharacterbook");
    if (gamecharacterbook) {
      const height = document.getElementById("main").getBoundingClientRect().height - gamecharacterbook.getBoundingClientRect().top;
      gamecharacterbook.style.height = `${height}px`;
    }
  }

  async function createVisualLayerAsync() {
    if (!_stage || !_game) {
      throw new Error("Must call initialize.");
    }

    _visualLayer.destroyChildren();

    if (!_game.imageUrl) {
      return;
    }

    // Render background image.
    let imageGroup = new Konva.Group({
      id: "imageGroup",
    });
    _visualLayer.add(imageGroup);

    let visualImage = await common.createImageAsync(_game.imageUrl);

    const width = _game.width * _game.tileWidth;
    const height = _game.height * _game.tileHeight;

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

    // TODO: Add on fog of war
    //for (let y = 0; y < _game.height; y++) {
    //  for (let x = 0; x < _game.width; x++) {
    //    let boosh = new Konva.Rect({
    //      x: _game.tileWidth * x,
    //      y: _game.tileHeight * y,
    //      width: _game.tileWidth,
    //      height: _game.tileHeight,
    //      fill: "#000000",
    //    });

    //    imageGroup.add(boosh);
    //  }
    //}

    // Render grid.
    let gridGroup = new Konva.Group({
      id: "grid",
      visible: _isGridVisible,
    });
    _visualLayer.add(gridGroup);

    // Create grid.
    for (let x = 0; x < _game.width; x++) {
      gridGroup.add(
        new Konva.Line({
          points: [Math.round(x * _game.tileWidth) + 0.5, 0, Math.round(x * _game.tileWidth) + 0.5, height],
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

    for (var y = 0; y < _game.height; y++) {
      gridGroup.add(
        new Konva.Line({
          points: [0, Math.round(y * _game.tileHeight), width, Math.round(y * _game.tileHeight)],
          stroke: GRID_COLOR,
          strokeWidth: 0.5,
          listening: false,
        }));
    }
  }

  async function createObjectLayerAsync() {
    if (!_stage || !_game) {
      throw new Error("Must call initialize.");
    }

    _objectLayer.destroyChildren();

    var characters = _gameState.gameCharacters.filter(c => c.isVisible);
    let previousPosition = {};

    for (let i = 0; i < characters.length; i++) {
      let character = characters[i];
      let node = await createCharacterNodeAsync(character);
      if (character.healthPoints === 0) {
        node.opacity(0.8);
      }

      let invalidUser = _clientState.userId !== character.userId && !_clientState.isOwner;

      if (_gameState.gameStateMode === 0 && !invalidUser) {
        let nodeHighlight = new Konva.Rect({
          x: node.x(),
          y: node.y(),
          fill: HIGHLIGHT_COLOR,
          opacity: 0.6,
          stoke: HIGHLIGHT_COLOR,
          stokeWidth: 3,
          visible: false,
          width: node.width(),
          height: node.height(),
        });

        let nodeTransformer = new Konva.Transformer({
          node: node,
          resizeEnabled: false,
          rotateEnabled: false,
          borderEnabled: false,
          visible: false,
        });
        const deleteNodeWidth = 16;
        let deleteNode = new Konva.Image({
          image: _deleteOverlay,
          x: nodeTransformer.width() - (deleteNodeWidth / 2),
          y: -(deleteNodeWidth / 2),
          width: deleteNodeWidth,
          height: deleteNodeWidth,
        });
        nodeTransformer.add(deleteNode);
        nodeTransformer.on("transform", e => {
          deleteNode.x(nodeTransformer.width());
        });

        deleteNode.on("click", e => {
          _deletedGameCharacterId = node.id();
        });
        deleteNode.on("mouseover", e => {
          nodeTransformer.moveToTop();
          nodeTransformer.show();
          common.setCursorToPointer();
          _objectLayer.batchDraw();
        });
        deleteNode.on("mouseout", e => {
          nodeTransformer.hide();
          common.setCursorToDefault();
          _objectLayer.batchDraw();
        });

        node.on("mouseover", e => {
          node.draggable(true);
          nodeTransformer.moveToTop();
          nodeTransformer.show();
          _objectLayer.batchDraw();
        });
        node.on("mouseout", e => {
          node.draggable(false);
          nodeTransformer.hide();
          _objectLayer.batchDraw();
        });
        node.on("dragstart", e => {
          previousPosition = {
            x: node.x(),
            y: node.y(),
          };
          nodeHighlight.moveToTop();
          nodeHighlight.show();
          node.moveToTop();
          nodeTransformer.moveToTop();
        });
        node.on("dragend", e => {
          nodeHighlight.hide();

          let coordinates = getMouseCoordinates();

          // Undo dragging if out of bounds.
          if (coordinates.x < 0 || coordinates.x >= _game.width || coordinates.y < 0 || coordinates.y >= _game.height) {
            node.x(previousPosition.x);
            node.y(previousPosition.y);
          }

          previousPosition = {};

        });
        node.on("dragmove", e => {
          const mouseCoordinates = getMouseCoordinates();
          const dx = common.clamp(mouseCoordinates.x, 0, _game.width - 1);
          const dy = common.clamp(mouseCoordinates.y, 0, _game.height - 1);

          nodeHighlight.x(dx * _game.tileWidth);
          nodeHighlight.y(dy * _game.tileHeight);

          _draggingGameCharacter = {
            x: nodeHighlight.x(),
            y: nodeHighlight.y(),
            id: node.id(),
          };
        });

        _objectLayer.add(nodeHighlight);
        _objectLayer.add(nodeTransformer);
      }

      _objectLayer.add(node);
    }
  }

  function createFogOfWarLayer() {
    if (!_stage || !_game) {
      throw new Error("Must call initialize.");
    }

    _fogOfWarLayer.destroyChildren();

    const mapWidth = _game.width;
    const mapHeight = _game.height;

    const tileWidth = _game.tileWidth;
    const tileHeight = _game.tileHeight;

    let characters = _gameState.gameCharacters.filter(c => c.isVisible);

    for (let y = 0; y < mapHeight; y++) {
      for (let x = 0; x < mapWidth; x++) {
        let node = new Konva.Rect({
          x: x * tileWidth,
          y: y * tileHeight,
          width: tileWidth,
          height: tileHeight,
          fill: "#2F2F2F",
        });

        _fogOfWarLayer.add(node);
      }
    }
  }
 
  async function createCharacterNodeAsync(character) {
    if (!character.imageUrl) {
      character.imageUrl = "/assets/question-mark.png";
    }

    let characterImage = await common.createImageAsync(character.imageUrl);

    let isSelected = false;
    if (_clientState) {
      if (_clientState.selectedGameCharacterId === character.id) {
        isSelected = true;
      }
    }

    let characterNode = new Konva.Image({
      image: characterImage,
      x: character.x * _game.tileWidth,
      y: character.y * _game.tileHeight,
      width: character.width * _game.tileWidth,
      height: character.height * _game.tileHeight,
      draggable: false,
      id: character.id,
      name: JSON.stringify(character),
      stroke: TARGET_COLOR,
      strokeWidth: 2,
      strokeEnabled: isSelected,
      dash: [21, 21, 43, 21, 43, 21, 43, 21, 43],
    });


    characterNode.on("mouseover", e => {
      common.setCursorToPointer();
    });
    characterNode.on("mouseout", e => {
      common.setCursorToDefault();
    });

    return characterNode;
  }
  function createAttackShapeFromMouseCoordinate() {
    let sourceCoordinate = _clientState.origin;
    let mouseCoordinate = getMouseCoordinates();
    if (mouseCoordinate.x < 0 || mouseCoordinate.x >= _game.width || mouseCoordinate.y < 0 || mouseCoordinate.y >= _game.height) {
      return;
    }

    // TODO: implement.
    let nodes = [];
    nodes.push(new Konva.Rect({
      x: mouseCoordinate.x * _game.tileWidth,
      y: mouseCoordinate.y * _game.tileHeight,
      width: _game.tileWidth,
      height: _game.tileHeight,
      fill: HIGHLIGHT_COLOR,
      opacity: 0.8,
    }));

    return nodes;
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
      x: Math.floor(mousePosition.x / _game.tileWidth),
      y: Math.floor(mousePosition.y / _game.tileHeight),
    };
  }

  window.game = {
    initialize: function () {
      window.requestAnimationFrame(async () => {
        _deleteOverlay = await common.createImageAsync("/assets/close-circle-red-512.png");
        _isGridVisible = true;

        const canvasDimensions = getCanvasDimensions();

        // Load main container.
        _stage = new Konva.Stage({
          container: "gameContainer",
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

        _visualLayer = new Konva.Layer();
        _objectLayer = new Konva.Layer();
        _interactiveLayer = new Konva.Layer({
          opacity: 0.5,
        });
        _fogOfWarLayer = new Konva.Layer({
          //opacity: 0.98,
        });

        _stage.add(_visualLayer);
        _stage.add(_objectLayer);
        _stage.add(_interactiveLayer);
        _stage.add(_fogOfWarLayer);

        window.addEventListener("resize", e => {
          window.requestAnimationFrame(() => {
            const canvasDimensions = getCanvasDimensions();

            _stage.width(canvasDimensions.width);
            _stage.height(canvasDimensions.height);

            resizeUI();
          });
        });
        _stage.on("mousemove", e => {
          // TODO: replace
          //if (_mapMenuOption !== 0 && _mapMenuOption !== 1) {
          //  return;
          //}
          if (!_game) {
            return;
          }

          if (!_stage.getPointerPosition()) {
            return;
          }

          let mousePosition = getMousePosition();
          let mouseCoordinates = getMouseCoordinates();
          mouseCoordinates = {
            x: common.clamp(mouseCoordinates.x, 0, _game.width - 1),
            y: common.clamp(mouseCoordinates.y, 0, _game.height - 1),
          };

          // Render placement node.
          if (_placementNode) {

            _highlightPlacementNode.x(mouseCoordinates.x * _game.tileWidth);
            _highlightPlacementNode.y(mouseCoordinates.y * _game.tileHeight);
            _highlightPlacementNode.moveToTop();

            _placementNode.x(mousePosition.x - (_placementNode.width() / 2));
            _placementNode.y(mousePosition.y - (_placementNode.height() / 2));
            _placementNode.moveToTop();

            _interactiveLayer.batchDraw();
          }


          // Render cursor.
          if (_clientState) {
            if (_clientState.mode !== 0) {
              let cursorGroup = _interactiveLayer.getChildren(c => c.id() === "cursorshape")[0];
              cursorGroup.destroyChildren();

              if (_clientState.mode === 1) {
                let mouseCoordinate = getMouseCoordinates();

                cursorGroup.add(new Konva.Rect({
                  x: mouseCoordinate.x * _game.tileWidth,
                  y: mouseCoordinate.y * _game.tileHeight,
                  width: _game.tileWidth,
                  height: _game.tileHeight,
                  fill: HIGHLIGHT_COLOR,
                  opacity: 0.8,
                }));
              }

              // Render attack tiles around cursor.
              if (_clientState.mode === 2) {
                createAttackShapeFromMouseCoordinate().forEach(n => cursorGroup.add(n));
              }

              _interactiveLayer.batchDraw();
            }
          }
        });
      });
    },

    toggleGrid: function (isVisible) {
      let gridGroup = _visualLayer.getChildren(c => c.id() === "grid")[0];
      gridGroup.visible(isVisible);

      _visualLayer.batchDraw();

      _isGridVisible = isVisible;
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

      const offsetWidth = _stage.width() - ((_game.width * _game.tileWidth) * _stage.scaleX());
      const offsetHeight = _stage.height() - ((_game.height * _game.tileHeight) * _stage.scaleY());

      _stage.position({
        x: common.clamp(newPosition.x, offsetWidth, 0),
        y: common.clamp(newPosition.y, offsetHeight, 0),
      });
      _stage.batchDraw();
    },
    renderGame: function (game) {
      window.requestAnimationFrame(async () => {

    // Redo movement/attack tiles visuals, should not be overriden by server.
    // Right click should cancel everything.
    // Move placement node into interavtive layer to also not get overridden.
        _game = game;

        await createVisualLayerAsync();

        //await createObjectLayerAsync();

        _stage.batchDraw();
      });
    },
    renderGameState: function (gameState) {
      _gameState = gameState;

      window.requestAnimationFrame(async () => {
        await createObjectLayerAsync();


        _stage.batchDraw();
      });
    },
    renderClientState: function (clientState) {
      _clientState = clientState;

      window.requestAnimationFrame(async () => {
        // Clean up.
        _interactiveLayer.destroyChildren();
        _interactiveLayer.add(new Konva.Group({
          id: "cursorshape",
        }));
        _placementNode = null;
        _highlightPlacementNode = null;
        _selectedHightlightNode = null;

        _interactiveLayer.batchDraw();

        // Render placement node.
        if (clientState.placementGameCharacter) {
          _placementNode = await createCharacterNodeAsync(clientState.placementGameCharacter);
          _highlightPlacementNode = new Konva.Rect({
            fill: HIGHLIGHT_COLOR,
            opacity: 0.6,
            stoke: HIGHLIGHT_COLOR,
            stokeWidth: 3,
            width: _placementNode.width(),
            height: _placementNode.height(),
          });

          _interactiveLayer.add(_highlightPlacementNode);
          _interactiveLayer.add(_placementNode);
        }

        // Render coordinates.
        if (clientState.coordinates.length > 0) {

          //// Testing
          //let neighborCoordinates = [
          //  { x: 0, y: -1 },
          //  { x: 1, y: 0 },
          //  { x: 0, y: 1 },
          //  { x: -1, y: 0 },

          //  { x: -1, y: -1 },
          //  { x: -1, y: 1 },
          //  { x: 1, y: 1 },
          //  { x: 1, y: -1 },
          //];

          //let visitedTiles = [];
          //let queue = [];
          //queue.push({
          //  x: 20,
          //  y: 20,
          //  v: 0,
          //});

          //while (queue.length > 0) {
          //  let tile = queue[0];
          //  queue.splice(0, 1);

          //  neighborCoordinates
          //    .map(c => {
          //      return {
          //        x: c.x + tile.x,
          //        y: c.y + tile.y,
          //        v: tile.v + (Math.abs(c.x) === Math.abs(c.y) ? 3 : 2),
          //      }
          //    })
          //    .filter(c => {
          //      if (visitedTiles.some(t => t.x === c.x && t.y === c.y))
          //        return false;

          //      if (c.x < 0 || c.x >= _game.width || c.y < 0 || c.y >= _game.height)
          //        return false;

          //      if (c.v > (2 * 100) + 1)
          //        return false;

          //      return true;
          //    })
          //    .forEach(c => {
          //      console.log(c.x + " " + c.y);
          //      visitedTiles.push({
          //        x: c.x,
          //        y: c.y,
          //        v: c.v,
          //      });

          //      queue.push({
          //        x: c.x,
          //        y: c.y,
          //        v : c.v,
          //      });
          //    });
          //}

          //clientState.coordinates = visitedTiles;

          clientState.coordinates.forEach(c => {
            let node = new Konva.Rect({
              x: c.x * _game.tileWidth,
              y: c.y * _game.tileHeight,
              fill: clientState.mode === 1 ? MOVE_COLOR : TARGET_COLOR,
              width: _game.tileWidth,
              height: _game.tileHeight,
            });
            _interactiveLayer.add(node);
          });
        }

        _interactiveLayer.batchDraw();

        if (_objectLayer) {
          if (_previousSelectedGameCharacterId) {
            let node = _objectLayer.getChildren(c => c.id() === _previousSelectedGameCharacterId);
            if (node) {
              node.strokeEnabled(false);
              _previousSelectedGameCharacterId = null;
            }
          }

          // Render selected character
          if (_clientState.selectedGameCharacterId) {
            let node = _objectLayer.getChildren(c => c.id() === _clientState.selectedGameCharacterId);
            if (node) {
              node.strokeEnabled(true);
              _previousSelectedGameCharacterId = _clientState.selectedGameCharacterId;
            }
          }

          _objectLayer.batchDraw();
        }

      });
    },
    getCoordinates: function () {
      return getMouseCoordinates();
    },
    getGameContainerPosition: function () {
      let bounds = document.getElementById("gameContainer").getBoundingClientRect();
      let top = document.getElementById("menu").getBoundingClientRect().top;

      return {
        x: Math.round(bounds.left),
        y: Math.round(top),
      };
    },
    resizeCanvas: function () {
      window.requestAnimationFrame(() => {
        const canvasDimensions = getCanvasDimensions();

        _stage.width(canvasDimensions.width);
        _stage.height(canvasDimensions.height);
      });
    },
    reset: function () {
      // TODO: Undo all visual interactions performed, this should show only the visual and object layers.
      _gameState = 0;
      _interactiveLayer.destroyChildren();
      _interactiveLayer.batchDraw();

      if (_placementNode) {
        _placementNode.destroy();
        _placementNode = null;
      }

      if (_highlightPlacementNode) {
        _highlightPlacementNode.destroy();
        _highlightPlacementNode = null;
      }

      _objectLayer.batchDraw();
    },
    requestMovement: function (coordinates) {
      _interactiveLayer.destroyChildren();
      _interactiveLayer.add(new Konva.Group({
        id: "attackshape",
      }));

      coordinates.forEach(c => {
        let node = new Konva.Rect({
          x: c.x * _game.tileWidth,
          y: c.y * _game.tileHeight,
          fill: MOVE_COLOR,
          width: _game.tileWidth,
          height: _game.tileHeight,
        });

        let testing = new Konva.Text({
          x: c.x * _game.tileWidth,
          y: c.y * _game.tileHeight,
          text: c.value,
        });

        _interactiveLayer.add(node);
        _interactiveLayer.add(testing);
      });

      _interactiveLayer.batchDraw();
    },
    setPlacementGameCharacter: async function (gameCharacterId) {
      if (_placementNode) {
        _placementNode.destroy();
        _placementNode = null;
      }

      if (_highlightPlacementNode) {
        _highlightPlacementNode.destroy();
        _highlightPlacementNode = null;
      }

      _objectLayer.batchDraw();

      let gameCharacter = _gameCharacters.find(c => c.id == gameCharacterId);
      if (!gameCharacter) {
        return;
      }

      _placementNode = await createCharacterNodeAsync(gameCharacter);
      _highlightPlacementNode = new Konva.Rect({
        fill: HIGHLIGHT_COLOR,
        opacity: 0.6,
        stoke: HIGHLIGHT_COLOR,
        stokeWidth: 3,
        width: _placementNode.width(),
        height: _placementNode.height(),
      });

      _objectLayer.add(_highlightPlacementNode);
      _objectLayer.add(_placementNode);
    },

    getDeletedCharacterId: function () {
      const characterId = _deletedGameCharacterId;
      _deletedGameCharacterId = null;

      return characterId;
    },
    getDraggingCharacter: function () {
      const character = _draggingGameCharacter;
      _draggingGameCharacter = null;
      if (!character) {
        return null;
      }

      return {
        x: Math.floor(character.x / _game.tileWidth),
        y: Math.floor(character.y / _game.tileHeight),
        id: character.id,
      };
    },
    getPlacementCharacterId: function () {
      if (!_placementNode) {
        return null;
      }

      return _placementNode.id();
    },
    setGameState: function (state) {
      _gameState = state;
    },
    resizeUI: function () {
      window.requestAnimationFrame(() => {
        resizeUI();
      });
    },
  };
})();