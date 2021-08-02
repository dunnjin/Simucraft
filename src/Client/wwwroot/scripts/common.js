(function () {
  window.common = {
    clamp: function (value, min, max) {
      return Math.min(Math.max(value, min), max);
    },
    createImageAsync: function (imageSource) {
      return new Promise((resolve, reject) => {
        let image = new Image();
        image.src = imageSource;
        image.onload = () => resolve(image);
        image.onerror = reject;
      });
    },
    setCursorToPointer: function() {
      document.body.style.cursor = 'pointer';
    },
    setCursorToDefault: function() {
      document.body.style.cursor = 'default';
    },
    createGuidAsync: async function() {
      return await DotNet.invokeMethodAsync("Simucraft.Client", 'CreateGuid');
    },
    resizeImage: function (imageReference, mediaType, width, height) {
      let canvas = document.createElement("canvas");
      let context = canvas.getContext("2d");
      context.drawImage(imageReference, 0, 0, width, height);
      return canvas.toDataURL(mediaType);
    },
    scrollToBottom: function (elementId) {
      let element = document.getElementById(elementId);
      element.scrollTop = element.scrollHeight;
    },
  };
})();