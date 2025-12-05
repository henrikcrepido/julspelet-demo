// Dice Animation JavaScript Interop
// Provides precise timing control for dice rolling animations with enhanced sprite strip effect

window.diceAnimation = {
    // Duration of the roll animation in milliseconds
    rollDuration: 600,
    
    // Performance optimization: use requestAnimationFrame
    animationFrameId: null,
    
    // Sound event callbacks (to be implemented by game)
    onRollStart: null,
    onRollBounce: null,
    onRollComplete: null,
    
    // Starts the dice rolling animation with sprite strip effect
    startRoll: function(diceCount) {
        console.log(`Starting enhanced dice roll animation for ${diceCount} dice`);
        
        // Trigger sound event hook
        if (this.onRollStart) {
            this.onRollStart(diceCount);
        }
        
        // Add visual feedback that roll has started
        const diceElements = document.querySelectorAll('.dice-rolling');
        diceElements.forEach((die, index) => {
            // Trigger reflow to restart animation if needed
            die.style.animation = 'none';
            setTimeout(() => {
                die.style.animation = '';
            }, 10);
        });
        
        // Add bounce sound events at key moments
        setTimeout(() => {
            if (this.onRollBounce) this.onRollBounce(1);
        }, 120);
        
        setTimeout(() => {
            if (this.onRollBounce) this.onRollBounce(2);
        }, 280);
        
        setTimeout(() => {
            if (this.onRollBounce) this.onRollBounce(3);
        }, 460);
    },
    
    // Waits for the animation to complete
    waitForAnimation: function() {
        return new Promise((resolve) => {
            setTimeout(() => {
                console.log('Dice roll animation completed');
                
                // Trigger completion sound event
                if (this.onRollComplete) {
                    this.onRollComplete();
                }
                
                resolve();
            }, this.rollDuration);
        });
    },
    
    // Plays a celebration animation for high-value rolls
    celebrateHighRoll: function(diceIndices) {
        diceIndices.forEach(index => {
            const diceElement = document.querySelector(`[data-dice-index="${index}"]`);
            if (diceElement) {
                diceElement.classList.add('high-roll-celebration');
                setTimeout(() => {
                    diceElement.classList.remove('high-roll-celebration');
                }, 1000);
            }
        });
    },
    
    // Adds a subtle shake to held dice when trying to roll
    shakeHeldDice: function() {
        const heldDice = document.querySelectorAll('.dice-held');
        heldDice.forEach(die => {
            die.classList.add('shake-held');
            setTimeout(() => {
                die.classList.remove('shake-held');
            }, 500);
        });
    },
    
    // Adds motion blur effect during roll for enhanced realism
    addMotionBlur: function() {
        const rollingDice = document.querySelectorAll('.dice-rolling');
        rollingDice.forEach(die => {
            die.style.filter = 'blur(2px)';
            setTimeout(() => {
                die.style.filter = '';
            }, this.rollDuration * 0.7);
        });
    }
};
