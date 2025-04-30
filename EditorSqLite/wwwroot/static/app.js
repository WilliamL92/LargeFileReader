document.addEventListener('input', function (event) {
    if (event.target.tagName === 'TEXTAREA') {
        const textarea = event.target;
        if (textarea.scrollHeight > textarea.offsetHeight) {
            textarea.value = textarea.value.slice(0, -1); // Supprime le dernier caractère
        }
    }
});
function initializeTextArea(textareaId) {
    console.log(`Initialisation du textarea avec l'ID : ${textareaId}`);
    const textarea = document.getElementById(textareaId);

    if (textarea) {
        textarea.addEventListener('paste', function (event) {
            event.preventDefault();

            // Sauvegarder la position du curseur et la position de défilement
            const cursorPosition = textarea.selectionStart;
            const scrollPosition = textarea.scrollTop;

            const clipboardData = (event.clipboardData || window.clipboardData).getData('text');
            const pastedText = clipboardData.replace(/\r?\n|\r/g, '\n');
            const currentText = textarea.value;

            const textBefore = currentText.substring(0, cursorPosition);
            const textAfter = currentText.substring(cursorPosition);
            const combinedText = textBefore + pastedText + textAfter;

            const lineHeight = parseInt(window.getComputedStyle(textarea).lineHeight, 10);
            const maxVisibleLines = Math.floor(textarea.offsetHeight / lineHeight);
            const lines = combinedText.split('\n');
            const visibleLines = lines.slice(-maxVisibleLines);

            // Mettre à jour la valeur du textarea
            textarea.value = visibleLines.join('\n');

            // Restaurer la position du curseur et la position de défilement
            textarea.selectionStart = textarea.selectionEnd = cursorPosition + pastedText.length;
            textarea.scrollTop = scrollPosition;

            console.log("Texte après collage :" + textarea.value);
        });
    } else {
        console.error(`Textarea avec l'ID ${textareaId} introuvable.`);
    }
}

