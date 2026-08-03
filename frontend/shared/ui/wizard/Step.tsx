import {
	Children,
	cloneElement,
	isValidElement,
	memo,
	ReactElement,
	ReactNode,
} from "react"
import StepContent from "./StepContent"
import StepHeader from "./StepHeader"

type StepChildProps = {
	stepId?: number | string
	children?: ReactNode
	disabled?: boolean
}

const Step = ({
	children,
	stepId,
	disabled,
}: {
	children?: ReactNode
	stepId?: number | string
	disabled?: boolean
}) => {
	return (
		<>
			{Children.map(children, (child) => {
				if (!isValidElement(child)) {
					return child
				}

				// прокидывает одинаковый stepId и disabled для StepHeader и StepContent, чтобы связать их
				if (child.type === StepHeader || child.type === StepContent) {
					return cloneElement(child as ReactElement<StepChildProps>, {
						stepId,
						disabled,
					})
				}

				return child
			})}
		</>
	)
}
export default memo(Step)
