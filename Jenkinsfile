pipeline {
    agent {
        label 'windows'
    }

    stages {
        stage('Cleanup') {
            fileOperations([fileDeleteOperation(includes: './*.zip')])
        }

        stage('Build') {
            steps {
                checkout scm
                powershell './build.ps1 -Target Pack -Configuration Release'
            }
        }

        stage('Archive') {
            steps {
                archiveArtifacts '/*.zip'
            }
        }
    }
}
