function setCountdown(){
    const quizDuration = 1000 * 60 *30 ; // 30 minutes
    
    // Get the starting time from localStorage, or set it to the current time if it's not set yet
    let startTime = localStorage.getItem('quizStartTime') || new Date().getTime();

    // Update the stored starting time in localStorage
    localStorage.setItem('quizStartTime', startTime);

    // Define the function to update the timer display
    function updateTimer() {
        let currentTime = new Date().getTime();
        let elapsedTime = currentTime - startTime;
        let secondsRemaining = Math.max(0, Math.floor((quizDuration - elapsedTime) / 1000));
        // If the time is up, submit the form
        if (secondsRemaining === 0) {
            localStorage.removeItem('quizStartTime');
            submitQuestion(true);
        }        
        let minutesRemaining = Math.floor(secondsRemaining / 60);
        let secondsDisplay = secondsRemaining % 60;
        $("#timer").text(minutesRemaining.toString().padStart(2, '0') + ':' + secondsDisplay.toString().padStart(2, '0'));
    }

    // Call the updateTimer function every second
    setInterval(updateTimer, 1000);
}


// document ready
$(function () {
    $('#submitCheck').hide();
    setCountdown();
});

// submit button
$('#finishBtn').click(function () {
    $('#submitCheck').show();
});
$('#dontFinish').click(function () {
    $('#submitCheck').hide();
});

function submitQuestion(isFinish = false){
    const textQuestionForm = $('#text-q-a');
    let doChecks = !isFinish;
    if(textQuestionForm.is('form')){
        const textAnswer = $("#questionAnswer");
        if(doChecks && (textAnswer.val() === "" || textAnswer.val() == null)){
            return;
        }
        if(isFinish){
            // add hidden input to form
            textQuestionForm.append('<input type="hidden" name="isSubmit" value="1" />');
        }
        textQuestionForm.submit();
    }else{
        
        const mcqForm = $('#mcq-a');
        const mcqAnswer = $("input[name='questionAnswer']:checked");
        if(doChecks && (mcqAnswer.val() === "" || mcqAnswer.val() == null)){
            return;
        }
        if(isFinish) {
            // add hidden input to form
            textQuestionForm.append('<input type="hidden" name="isSubmit" value="1" />');
        }
        mcqForm.submit();
    }

};

$("#next-btn").click(()=>{
    submitQuestion();
});

const submitHandler = () => {
    localStorage.removeItem('quizStartTime');
    submitQuestion(true);
};

$("#submit-btn").click(()=> {
    submitHandler();
});

$("#submit-btn-2").click(()=> {
    submitHandler();
});