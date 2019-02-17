pipeline {
    agent {
        label 'windows'
    }

    stages {
        stage('Cleanup') {
            steps {
                fileOperations([fileDeleteOperation(includes: '*.zip')])
            }
        }

        stage('Build') {
            steps {
                checkout scm
                powershell './build.ps1 -Target Pack -Configuration Release'
                nunit testResultsPattern: 'OnTheGoPlayer.Test/bin/Debug/TestResult.xml'
            }
        }

        stage('Archive') {
            steps {
                archiveArtifacts '/*.zip'
            }
        }
    }
}
