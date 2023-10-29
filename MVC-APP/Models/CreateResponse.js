<script>
    document.getElementById("addTemplate").addEventListener
    ("click", function () 
    {
        var selectedTemplate = document.getElementById("Template").value;
        var textarea = document.getElementById("response");
        
        // Insert the selected template at the current cursor position
        var start = textarea.selectionStart;
        var end = textarea.selectionEnd;
        var text = textarea.value;
        var before = text.substring(0, start);
        var after = text.substring(end, text.length);
        var template = EmailTemplates[selectedTemplate];
        textarea.value = before + template + after;
        }
        );
</script>
